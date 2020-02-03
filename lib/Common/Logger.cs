using Common.Commands;
using Common.Config;
using System;
using System.Collections.Generic;
using System.IO;

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
		public static LogEvent OnLog;
		public static ELogLevel CurrentLogLevel = ELogLevel.Info;
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
		}

		public static void Exception(Exception exception, string message = null)
		{
			if (exception == null)
			{
				return;
			}
			if (!string.IsNullOrEmpty(message))
			{
				Error(message, true);
			}
			Error(exception.Message, true);
			Error(exception.StackTrace, true);
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
				message = $"[{DateTime.Now.ToString()}]\t{message}";
				Console.ForegroundColor = foreground;
				Console.BackgroundColor = background;
				Console.WriteLine(message);
				OnLog?.Invoke(message, logLevel);
			}
			catch (Exception e)
			{
				Console.WriteLine(e.Message);
			}
			finally
			{
				Console.ForegroundColor = prevFore;
				Console.BackgroundColor = prevBack;
			}
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
