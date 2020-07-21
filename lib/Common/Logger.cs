using Common.Commands;
using Common.Config;
using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Reflection;
using System.Runtime.CompilerServices;
using System.Text;

namespace Common
{
	public enum ELogLevel
	{
		Error,
		Warning,
		Info,
		Debug,
	}

	public readonly struct LogMessage
	{
		public readonly DateTime Timestamp;
		public readonly ELogLevel LogLevel;
		public readonly string Source;
		public readonly string Message;

		public LogMessage(string message, ELogLevel logLevel, string category, DateTime now) : this()
		{
			Message = message;
			LogLevel = logLevel;
			Timestamp = now;
			Source = category;
		}

		public override string ToString()
		{
			return $"{Timestamp}\t{LogLevel}\t[{Source}]\t{Message}";
		}
	}

	public delegate void LogEvent(LogMessage message);

	public static class Logger
	{
		private const string PARAM_Y = "y";

		public static LogEvent OnLog;

		public static ELogLevel CurrentLogLevel
		{
			get
			{
				return __logLevel;
			}
			set
			{
				if(__logLevel == value)
				{
					return;
				}
				if(value < ELogLevel.Info)
				{
					Warning($"Can't set log level lower than {nameof(ELogLevel.Info)}");
					return;
				}
				Info($"Set log level to {value}");
				__logLevel = value;
			}
		}
		private static ELogLevel __logLevel;
		public static bool PromptUser { get; private set; }
		private static object m_fileLock = new object();

		static Logger()
		{
			__logLevel = ELogLevel.Debug;
			CurrentLogLevel = new ConfigVariable<ELogLevel>("LogLevel", ELogLevel.Debug).Value;
			CommandManager.OnPreExecute += GetArgs;
		}

		private static void GetArgs(CommandArguments args)
		{
			var newLog = args.TryGetValue("log", CurrentLogLevel);
			if (newLog != CurrentLogLevel)
			{
				CurrentLogLevel = newLog;
				Info($"Logging level is " + CurrentLogLevel);
			}
			PromptUser = !args.HasKey(PARAM_Y);
		}

		public static void Exception(Exception exception, string message = null)
		{
			if (exception == null)
			{
				return;
			}
			var sb = new StringBuilder(message);
			if (!string.IsNullOrEmpty(message))
			{
				sb.Append('\n');
			}
			if(exception is TargetInvocationException)
			{
				exception = exception.InnerException;
			}
			sb.AppendLine(exception.Message);
			sb.AppendLine(exception.StackTrace);
			DoLog(sb.ToString(), ConsoleColor.Red, ConsoleColor.Black, ELogLevel.Error, "EXCEPTION");
			if (exception.InnerException != null)
			{
				Exception(exception.InnerException, "Inner exception: ");
			}
		}

		public static void Info(string message)
		{
			DoLog(message, Console.ForegroundColor, Console.BackgroundColor, ELogLevel.Info, GetSource());
		}

		//[Conditional("DEBUG")] todo
		public static void Debug(string message)
		{
			DoLog(message, Console.ForegroundColor, Console.BackgroundColor, ELogLevel.Debug, GetSource());
		}

		private static void DoLog(string message, ConsoleColor foreground, ConsoleColor background, ELogLevel logLevel, string category)
		{
			if (logLevel > CurrentLogLevel)
			{
				return;
			}
			ConsoleColor prevFore = Console.ForegroundColor;
			ConsoleColor prevBack = Console.BackgroundColor;
			try
			{
				var msg = new LogMessage(message, logLevel, category, DateTime.Now);
				Console.ForegroundColor = foreground;
				Console.BackgroundColor = background;
				if(logLevel == ELogLevel.Error)
				{
					Console.Error.WriteLine(msg.ToString());
				}
				else
				{
					Console.WriteLine(msg.ToString());
				}
				OnLog?.Invoke(msg);
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.Message);
			}
			finally
			{
				Console.ForegroundColor = prevFore;
				Console.BackgroundColor = prevBack;
			}
		}

		public static bool Prompt(string message, ConsoleKey confirm = ConsoleKey.Y)
		{
			const ConsoleKey ALWAYS = ConsoleKey.A;
			if(!PromptUser)
			{
				Info(message);
				return true;
			}
			ConsoleColor prevFore = Console.ForegroundColor;
			ConsoleColor prevBack = Console.BackgroundColor;
			try
			{
				while(Console.KeyAvailable) { Console.ReadKey(); /* flush buffer */ }
				message = $"{message}";
				Console.ForegroundColor = ConsoleColor.Green;
				Console.WriteLine(message);
				Console.WriteLine($"Press {confirm} to confirm, {ALWAYS} to confirm all further actions, or any other key to cancel.");
			}
			catch (Exception e)
			{
				Console.Error.WriteLine(e.Message);
			}
			finally
			{
				Console.ForegroundColor = prevFore;
				Console.BackgroundColor = prevBack;
			}
			var key = Console.ReadKey().Key;
			Console.WriteLine();
			if(key == ALWAYS)
			{
				PromptUser = false;
			}
			return key == confirm || key == ALWAYS;
		}

		public static void Error(string message, bool nolog = false)
		{
			DoLog(message, ConsoleColor.Red, ConsoleColor.Black, ELogLevel.Error, GetSource());
		}

		public static void Warning(string message)
		{
			DoLog(message, ConsoleColor.Yellow, ConsoleColor.Black, ELogLevel.Warning, GetSource());
		}

		static string GetSource()
		{
			var topFrame = new StackTrace().GetFrames().FirstOrDefault(f => f.GetMethod().DeclaringType != typeof(Logger)).GetMethod();
			if(topFrame == null)
			{
				return null;
			}
			return $"{topFrame.DeclaringType.GetRealTypeName()}.{topFrame.Name}";
		}
	}
}
