using System;
using System.Security;

namespace Common
{
	public static class ConsoleExtensions
	{
		public static SecureString ReadPassword()
		{
			var pass = new SecureString();
			do
			{
				ConsoleKeyInfo key = Console.ReadKey(true);
				// Backspace Should Not Work
				if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
				{
					pass.AppendChar(key.KeyChar);
					Console.Write("*");
				}
				else
				{
					if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
					{
						pass.RemoveAt(pass.Length - 1);
						Console.Write("\b \b");
					}
					else if (key.Key == ConsoleKey.Enter)
					{
						break;
					}
				}
			} while (true);
			return pass;
		}
	}
}
