﻿using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
	public static class ConsoleExtensions
	{
		public static string ReadPassword()
		{
			string pass = "";
			do
			{
				ConsoleKeyInfo key = Console.ReadKey(true);
				// Backspace Should Not Work
				if (key.Key != ConsoleKey.Backspace && key.Key != ConsoleKey.Enter)
				{
					pass += key.KeyChar;
					Console.Write("*");
				}
				else
				{
					if (key.Key == ConsoleKey.Backspace && pass.Length > 0)
					{
						pass = pass.Substring(0, (pass.Length - 1));
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

	public static class JsonExtensions
	{
		public static T DeserializeAnonymousType<T>(string json, T anonymousObj) where T : class
		{
			var newObj = JsonConvert.DeserializeAnonymousType(json, anonymousObj);
			if(newObj == null)
			{
				throw new NullReferenceException();
			}
			return newObj;
		}
	}
}
