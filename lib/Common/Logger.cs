using Common.Commands;
using Common.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Reflection;
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

	public delegate void LogEvent(string message, ELogLevel logLevel);

	public static class Logger
	{
		private const string PARAM_Y = "y";

		public static LogEvent OnLog;
		public static ELogLevel CurrentLogLevel = ELogLevel.Info;
		public static bool PromptUser { get; private set; }
		private static object m_fileLock = new object();

		static Logger()
		{
			CommandManager.OnPreExecute += GetArgs;
			CurrentLogLevel = new ConfigVariable<ELogLevel>("LogLevel", ELogLevel.Info).Value;
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
			DoLog(sb.ToString(), ConsoleColor.Red, ConsoleColor.Black, ELogLevel.Error);
			if (exception.InnerException != null)
			{
				Exception(exception.InnerException, "Inner exception: ");
			}
		}

		public static void Info(string message)
		{
			DoLog(message, Console.ForegroundColor, Console.BackgroundColor, ELogLevel.Info);
		}

		public static void Debug(string message)
		{
			DoLog(message, Console.ForegroundColor, Console.BackgroundColor, ELogLevel.Debug);
		}

		private static void DoLog(string message, ConsoleColor foreground, ConsoleColor background, ELogLevel logLevel)
		{
			if (logLevel > CurrentLogLevel)
			{
				return;
			}
			ConsoleColor prevFore = Console.ForegroundColor;
			ConsoleColor prevBack = Console.BackgroundColor;
			try
			{
				//message = $"[{DateTime.Now.ToLocalTime().ToShortTimeString()}]\t{message}";
				Console.ForegroundColor = foreground;
				Console.BackgroundColor = background;
				if(logLevel == ELogLevel.Error)
				{
					Console.Error.WriteLine(message);
				}
				else
				{
					Console.WriteLine(message);
				}
				OnLog?.Invoke(message, logLevel);
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
			DoLog(message, ConsoleColor.Red, ConsoleColor.Black, ELogLevel.Error);
		}

		public static void Warning(string message)
		{
			DoLog(message, ConsoleColor.Yellow, ConsoleColor.Black, ELogLevel.Warning);
		}
	}
}
