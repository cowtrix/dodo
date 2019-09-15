using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public static class Logger
	{
		public static void Exception(Exception exception, string message = null)
		{
			if(!string.IsNullOrEmpty(message))
			{
				Logger.Debug(message);
			}
			Error(exception.Message);
			Error(exception.StackTrace);
		}

		public static void Debug(string message, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
		{
			Console.ForegroundColor = foreground;
			Console.BackgroundColor = background;
			Console.WriteLine(message);
		}

		public static void Error(string message)
		{
			Debug(message, ConsoleColor.Red);
		}
	}
}
