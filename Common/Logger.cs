using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Common
{
	public static class Logger
	{
		public static string LogPath = "dodoLog.log";
		private static object m_fileLock = new object();

		public static void Exception(Exception exception, string message = null)
		{
			if(!string.IsNullOrEmpty(message))
			{
				Error(message);
			}
			Error(exception.Message);
			Error(exception.StackTrace);
		}

		public static void Debug(string message, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
		{
			try
			{
				message = $"[{DateTime.Now.ToString()}]\t{message}";
				Console.ForegroundColor = foreground;
				Console.BackgroundColor = background;
				Console.WriteLine(message);
				lock (m_fileLock)
				{
					File.AppendAllText(LogPath, message + "\n");
				}
			}
			catch(Exception e)
			{
				Logger.Exception(e);
			}
		}

		public static void Error(string message)
		{
			Debug(message, ConsoleColor.Red);
		}

		public static void Warning(string message)
		{
			Debug(message, ConsoleColor.Yellow);
		}
	}
}
