using Common.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Commands
{
	public static class RegexMatches
	{
		public const string WINDOWS_PATH = "(^([a-z]|[A-Z]):(?=\\\\(?![\\0-\\37<>:\"/\\\\|?*])|\\/(?![\\0-\\37<>:\"/\\\\|?*])|$)|^\\\\(?=[\\\\\\/][^\\0-\\37<>:\"/\\\\|?*]+)|^(?=(\\\\|\\/)$)|^\\.(?=(\\\\|\\/)$)|^\\.\\.(?=(\\\\|\\/)$)|^(?=(\\\\|\\/)[^\\0-\\37<>:\"/\\\\|?*]+)|^\\.(?=(\\\\|\\/)[^\\0-\\37<>:\"/\\\\|?*]+)|^\\.\\.(?=(\\\\|\\/)[^\\0-\\37<>:\"/\\\\|?*]+))((\\\\|\\/)[^\\0-\\37<>:\"/\\\\|?*]+|(\\\\|\\/)$)*()$";
		public const string POSITIVE_NUMBER = @"^[+]?\d+([.]\d+)?$";
	}

	public interface ICommandArguments
	{
		T TryGetValue<T>(string key, T defaultValue);
	}

	public interface ICommandArgumentValidator
	{
		bool Validate(CommandArgumentParameters param, string key, string rawValue, out string error);
	}

	public class CommandArgumentRegexSchema : ICommandArgumentValidator
	{
		private Dictionary<string, ValueTuple<string, string>> m_regexMapping = new Dictionary<string, (string, string)>();
		private bool m_allowUnspecifiedArgs;

		public CommandArgumentRegexSchema(bool allowUnspecifiedArgs, params ValueTuple<string, string, string>[] values)
		{
			m_allowUnspecifiedArgs = allowUnspecifiedArgs;
			foreach (var kvp in values)
			{
				m_regexMapping[kvp.Item1] = (kvp.Item2, kvp.Item3);
			}
		}

		public bool Validate(CommandArgumentParameters param, string key, string rawValue, out string error)
		{
			if (!m_regexMapping.TryGetValue(key, out var regex))
			{
				error = $"No argument found with key {key}";
				return m_allowUnspecifiedArgs;
			}
			if (!Regex.IsMatch(rawValue, regex.Item1))
			{
				error = $"Regex mismatch: {param.CommandPrefix}{key}{param.CommandParamSeperator}{rawValue} did not match required regex. {regex.Item2}";
				return false;
			}
			error = null;
			return true;
		}

		public string GetHelpString(CommandArgumentParameters argParams)
		{
			var sb = new StringBuilder("");
			foreach (var mapping in m_regexMapping)
			{
				sb.AppendLine($"{argParams.CommandPrefix}{mapping.Key}{argParams.CommandParamSeperator}value\t{mapping.Value.Item2}");
			}
			return sb.ToString();
		}
	}

	public delegate bool ValidateArgumentDelegate(CommandArgumentParameters param, string key, string rawValue, out string error);

	public class CommandArgumentParameters
	{
		public static CommandArgumentParameters Default => new ConfigVariable<CommandArgumentParameters>("DefaultCommandArgumentParameters", new CommandArgumentParameters()).Value;
		public readonly char CommandParamSeperator;
		public readonly char CommandSeperator;
		public readonly char CommandPrefix;
		public readonly char QuotationChar;
		public ValidateArgumentDelegate OnValidate;

		public CommandArgumentParameters(ICommandArgumentValidator validator, char prefix = '/', char paramSeperator = ':', char commandSeperator = ' ', char quotationChar = '"')
			: this(validator.Validate, prefix, paramSeperator, commandSeperator, quotationChar)
		{
		}

		public CommandArgumentParameters(ValidateArgumentDelegate validator = null, char prefix = '/', char paramSeperator = ':', char commandSeperator = ' ', char quotationChar = '"')
		{
			OnValidate = validator;
			CommandPrefix = prefix;
			CommandParamSeperator = paramSeperator;
			CommandSeperator = commandSeperator;
			QuotationChar = quotationChar;
		}
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
		public string RawValue { get; private set; }

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
			RawValue = arg;
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

					if (state == eReadState.Seek)
					{
						if (c == parameters.CommandPrefix)
						{
							// We start reading the parameter key, but skip the prefix
							state = eReadState.ReadKey;
							//continue;
						}
						/*if (c != parameters.CommandSeperator && c != EOL)
						{
							throw new Exception();
						}*/
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
			catch (Exception e)
			{
				throw new Exception($"Args [{arg}] failed to parse. Unexpected character at index {charCounter}\n", e);
			}
			if (parameters.OnValidate == null)
			{
				return;
			}
			foreach (var parsedArg in m_args)
			{
				if (!parameters.OnValidate(parameters, parsedArg.Key, parsedArg.Value, out var error))
				{
					throw new Exception(error);
				}
			}
		}

		public T TryGetValue<T>(IEnumerable<string> keys, T defaultValue)
		{
			foreach (var key in keys)
			{
				if (!m_args.TryGetValue(key, out var rawVal))
				{
					continue;
				}
				if (!TryConvert<T>(rawVal, out var value))
				{
					throw new Exception($"Failed to parse {rawVal} to type {typeof(T).FullName}");
				}
				return value;
			}
			return defaultValue;
		}

		public T TryGetValue<T>(string key, T defaultValue)
		{
			return TryGetValue<T>(new[] { key }, defaultValue);
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
			if (t == typeof(bool))
			{
				if (bool.TryParse(str, out var boolresult))
				{
					result = boolresult;
				}
				else
				{
					result = true;
				}
				return true;
			}
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
			if (t.IsEnum)
			{
				if (int.TryParse(str, out var intVal))
				{
					result = Convert.ChangeType(intVal, t);
					return true;
				}
				result = Enum.Parse(t, str);
				return true;
			}
			result = default(object);
			return false;
		}
	}
}
