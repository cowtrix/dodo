using Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Commands
{
	public interface ICommandArguments
	{
		T TryGetValue<T>(string key, T defaultValue);
	}

	public struct CommandArgumentParameters
	{
		public static CommandArgumentParameters Default => new ConfigVariable<CommandArgumentParameters>("DefaultCommandArgumentParameters", new CommandArgumentParameters
		{
			CommandParamSeperator = ':',
			CommandSeperator = ' ',
			CommandPrefix = '/',
			QuotationChar = '"',
		}).Value;

		public char CommandParamSeperator;
		public char CommandSeperator;
		public char CommandPrefix;
		public char QuotationChar;
	}

	public struct CommandArguments : ICommandArguments
	{
		enum eReadState
		{
			Seek,
			ReadKey,
			ReadValue,
			ReadValueInQuotations,
		}

		const char EOL = '\0';
		private Dictionary<string, string> m_args;

		public CommandArguments(IEnumerable<string> args, CommandArgumentParameters parameters) :
			this(args.Aggregate("", (current, next) => current + (current == "" ? "" : " ") + next), parameters)
		{
		}

		public CommandArguments(IEnumerable<string> args) :
			this(args, CommandArgumentParameters.Default)
		{
		}

		public CommandArguments(string arg) : this(arg, CommandArgumentParameters.Default)
		{
		}

		public CommandArguments(string arg, CommandArgumentParameters parameters)
		{
			m_args = new Dictionary<string, string>();
			var sb = new StringBuilder();
			eReadState state = eReadState.Seek;

			string keyBuffer = null;
			int charCounter = 0;
			// Go through the string provided character by character
			try
			{
				for (charCounter = 0; charCounter <= arg.Length; charCounter++)
				{
					char c = charCounter == arg.Length ? EOL : (char)arg[charCounter];

					if (state == eReadState.Seek && c == parameters.CommandPrefix)
					{
						// We start reading the parameter key, but skip the prefix
						state = eReadState.ReadKey;
						continue;
					}
					else if (state == eReadState.ReadKey)
					{
						if (c == parameters.CommandSeperator || c == EOL)
						{
							// It's a flag (no value)
							m_args[sb.ToString()] = null;
							sb.Clear();
							state = eReadState.Seek;
						}
						else if (c == parameters.CommandParamSeperator)
						{
							// Switch to reading value
							state = eReadState.ReadValue;
							keyBuffer = sb.ToString();
							sb.Clear();
						}
						else
						{
							sb.Append(c);
						}
						continue;
					}
					else if (state == eReadState.ReadValue)
					{
						if (c == parameters.QuotationChar)
						{
							state = eReadState.ReadValueInQuotations;
						}
						else if (c == parameters.CommandSeperator || c == EOL)
						{
							// Finished reading command with value
							m_args[keyBuffer] = sb.ToString();
							sb.Clear();
							keyBuffer = null;
							state = eReadState.Seek;
						}
						else
						{
							sb.Append(c);
						}
						continue;
					}
					else if (state == eReadState.ReadValueInQuotations)
					{
						if (c == parameters.QuotationChar)
						{
							state = eReadState.Seek;
							// Finished reading command with value within quotations
							m_args[keyBuffer] = sb.ToString();
							sb.Clear();
							keyBuffer = null;
							state = eReadState.Seek;
						}
						else
						{
							sb.Append(c);
						}
						continue;
					}
				}
			}
			catch(Exception e)
			{
				throw new Exception($"Args [{arg}] failed to parse. Unexpected character at index {charCounter}\n", e);
			}
		}

		public T TryGetValue<T>(string key, T defaultValue)
		{
			if(!m_args.TryGetValue(key, out var rawVal))
			{
				return defaultValue;
			}
			if(TryConvert<T>(rawVal, out var value))
			{
				return value;
			}
			throw new Exception($"Failed to parse {rawVal} to type {typeof(T).FullName}");
		}

		private bool TryConvert<T>(string str, out T result)
		{
			var t = typeof(T);
			var success = TryConvert(str, t, out var objResult);
			result = (T)objResult;
			return success;
		}

		private bool TryConvert(string str, Type t, out object result)
		{
			if (t == typeof(string))
			{
				result = str;
				return true;
			}
			if (t == typeof(int))
			{
				result = int.Parse(str);
				return true;
			}
			if (t == typeof(double))
			{
				result = double.Parse(str);
				return true;
			}
			if (t == typeof(DateTime))
			{
				result = DateTime.Parse(str);
				return true;
			}
			if (t == typeof(TimeSpan))
			{
				result = TimeSpan.Parse(str);
				return true;
			}
			if (t == typeof(Guid))
			{
				result = Guid.Parse(str);
				return true;
			}
			result = default;
			return false;
		}
	}
}
