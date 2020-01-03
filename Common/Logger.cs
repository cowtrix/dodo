using Common.Config;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public enum ELogLevel
	{
		Info,
		Warn,
		Error,
		Debug,
	}

	internal struct ExceptionEntry
	{
		public string Message;
		public DateTime TimeStamp;
	}

	public static class Logger
	{
		private static ConfigVariable<ELogLevel> m_logLevel = new ConfigVariable<ELogLevel>("LogLevel", ELogLevel.Debug);
		internal static List<ExceptionEntry> ExceptionLog = new List<ExceptionEntry>();
		public static string LogPath = @"logs\log.log";
		private static object m_fileLock = new object();

		static Logger()
		{
			var logDir = Path.GetDirectoryName(LogPath);
			if (!Directory.Exists(logDir))
			{
				Directory.CreateDirectory(logDir);
			}
		}

		public static void Exception(Exception exception, string message = null, bool nolog = false)
		{
			if(exception == null)
			{
				return;
			}
			if(!string.IsNullOrEmpty(message))
			{
				Error(message, true);
			}
			if(!nolog)
			{
				ExceptionLog.Add(new ExceptionEntry()
				{
					Message = exception.Message,
					TimeStamp = DateTime.Now,
				});
			}
			Error(exception.Message, true);
			Error(exception.StackTrace, true);
			if (exception.InnerException != null)
			{
				Exception(exception.InnerException, "Inner exception: ", nolog);
			}
		}

		public static void Debug(string message, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black, bool writeToLog = true, ELogLevel lvl = ELogLevel.Info)
		{
			if(lvl > m_logLevel.Value)
			{
				return;
			}
			try
			{
				message = $"{(writeToLog ? "" : "~")}[{DateTime.Now.ToString()}]\t{message}";
				Console.ForegroundColor = foreground;
				Console.BackgroundColor = background;
				Console.WriteLine(message);
				System.Diagnostics.Debug.WriteLine(message, lvl.GetName());
				if (writeToLog)
				{
					lock (m_fileLock)
					{
						File.AppendAllText(LogPath, message + "\n");
					}
				}
			}
			catch(Exception e)
			{
				Logger.Exception(e);
			}
		}

		public static void Error(string message, bool nolog = false)
		{
			if (!nolog)
			{
				ExceptionLog.Add(new ExceptionEntry()
				{
					Message = message,
					TimeStamp = DateTime.Now,
				});
			}
			Debug(message, ConsoleColor.Red, lvl:ELogLevel.Error);
		}

		public static void Warning(string message)
		{
			Debug(message, ConsoleColor.Yellow, lvl: ELogLevel.Warn);
		}

		public static void Alert(string message)
		{
			ExceptionLog.Add(new ExceptionEntry()
			{
				Message = message,
				TimeStamp = DateTime.Now,
			});
			Debug(message, ConsoleColor.Cyan, lvl: ELogLevel.Info);
		}
	}
}
