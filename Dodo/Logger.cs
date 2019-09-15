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
			Logger.Debug(exception.Message, ConsoleColor.Red);
			Logger.Debug(exception.StackTrace, ConsoleColor.Red);
		}

		public static void Debug(string message, ConsoleColor foreground = ConsoleColor.White, ConsoleColor background = ConsoleColor.Black)
		{
			Console.ForegroundColor = foreground;
			Console.BackgroundColor = background;
			Console.WriteLine(message);
		}
	}
}
