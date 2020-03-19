using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Common.Commands
{
	public delegate bool ValidateArgumentDelegate(CommandArgumentParameters param, string key, string rawValue, out string error);

	/// <summary>
	/// Represents the parsed values from a raw commandline string
	/// </summary>
	public struct CommandArguments
	{
		private Dictionary<string, string> m_args;

		public CommandArguments(string args, CommandArgumentParameters parameters) :
			this(Split(args, parameters), parameters)
		{
		}

		public CommandArguments(IEnumerable<string> args) :
			this(args, CommandArgumentParameters.Default)
		{
		}

		public CommandArguments(string arg) : this(arg, CommandArgumentParameters.Default)
		{
		}

		public CommandArguments(IEnumerable<string> args, CommandArgumentParameters parameters)
		{
			m_args = new Dictionary<string, string>();
			foreach(var arg in args)
			{
				if(!arg.StartsWith(parameters.CommandPrefix.ToString()))
				{
					continue;
				}
				var index = arg.IndexOf(parameters.CommandParamSeperator);
				if(index < 0)
				{
					// This is a flag
					m_args[arg.Substring(1)] = "";
					continue;
				}
				var key = arg.Substring(1, index - 1);
				var value = arg.Substring(index + 1);
				m_args[key] = value;
			}
			if(parameters.OnValidate != null)
			{
				foreach (var parsedArg in m_args)
				{
					if (!parameters.OnValidate(parameters, parsedArg.Key, parsedArg.Value, out var error))
					{
						throw new Exception(error);
					}
				}
			}
		}

		/// <summary>
		/// Get a value with the given keys, and throw an exception if not found
		/// </summary>
		/// <typeparam name="T">The type to return</typeparam>
		/// <param name="keys">The keys we can match</param>
		/// <returns>An object of type T that matches the given keys</returns>
		public T MustGetValue<T>(IEnumerable<string> keys)
		{
			if(!HasKey(keys))
			{
				throw new Exception($"Missing required argument: {string.Join(", ", keys)}");
			}
			return TryGetValue<T>(keys, default);
		}

		/// <summary>
		/// Get a value with the given key, and throw an exception if not found
		/// </summary>
		/// <typeparam name="T">The type to return</typeparam>
		/// <param name="key">The key we can match</param>
		/// <returns>An object of type T that matches the given key</returns>
		public T MustGetValue<T>(string key)
		{
			return MustGetValue<T>(new [] { key });
		}

		/// <summary>
		/// Try to get a value with the given keys, and return the default value
		/// if none is found.
		/// </summary>
		/// <typeparam name="T">The type to return</typeparam>
		/// <param name="keys">The keys we can match</param>
		/// <param name="defaultValue">A default value</param>
		/// <returns>An object of type T that matches the given keys</returns>
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

		/// <summary>
		/// Try to get a value with the given keys, and return the default value
		/// if none is found.
		/// </summary>
		/// <typeparam name="T">The type to return</typeparam>
		/// <param name="key">The key we can match</param>
		/// <param name="defaultValue">A default value</param>
		/// <returns>An object of type T that matches the given keys</returns>
		public T TryGetValue<T>(string key, T defaultValue)
		{
			return TryGetValue<T>(new[] { key }, defaultValue);
		}

		private bool TryConvert<T>(string str, out T result)
		{
			var t = typeof(T);
			if(!TryConvert(str, t, out var objResult))
			{
				throw new Exception($"Could not convert type {t}");
			}
			result = (T)objResult;
			return true;
		}

		private bool TryConvert(string str, Type t, out object result)
		{
			var nullType = Nullable.GetUnderlyingType(t);
			if (nullType != null)
			{
				// Nullable type extra handling
				return TryConvert(str, nullType, out result);
			}
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
			if (t == typeof(ulong))
			{
				result = ulong.Parse(str);
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
			if (typeof(IEnumerable).IsAssignableFrom(t))
			{
				var listGenericArg = t.GetGenericArguments().Single();
				if(t.IsInterface)
				{
					t = typeof(List<>).MakeGenericType(listGenericArg);
				}
				var list = Activator.CreateInstance(t) as IList;
				foreach (var obj in str.Split(new[] {","}, StringSplitOptions.RemoveEmptyEntries)
					.Select(x => x.Trim()))
				{
					if (!TryConvert(obj, listGenericArg, out var innerObj))
					{
						result = null;
						return false;
					}
					list.Add(innerObj);
				}
				result = list;
				return true;
			}
			result = default(object);
			return false;
		}

		/// <summary>
		/// Did the user explictly define the given key?
		/// </summary>
		/// <param name="key">The key to match</param>
		/// <returns>True if the user has given the key in the commandline</returns>
		public bool HasKey(string key)
		{
			return m_args.ContainsKey(key);
		}

		/// <summary>
		/// Did the user explictly define any of the given keys?
		/// </summary>
		/// <param name="keys">The keys to match</param>
		/// <returns>True if the user has given any of the keys in the commandline</returns>
		public bool HasKey(IEnumerable<string> keys)
		{
			var args = m_args;
			return keys.Any(key => args.ContainsKey(key));
		}

		/// <summary>
		/// Utility function to split up raw strings in string arrays, like Main(string[] args)
		/// </summary>
		/// <param name="raw">The raw commandline string</param>
		/// <param name="parameters">The parameters of how to parse the commandline</param>
		/// <returns>An enumerable of parsed commands</returns>
		public static IEnumerable<string> Split(string raw, CommandArgumentParameters parameters)
		{
			StringBuilder sb = new StringBuilder();
			var result = new List<string>();
			bool withinQuotes = false;
			foreach(var c in raw)
			{
				if(c == parameters.QuotationChar)
				{
					withinQuotes = !withinQuotes;
					continue;
				}

				if(c == parameters.CommandSeperator && !withinQuotes)
				{
					result.Add(sb.ToString());
					sb.Clear();
					continue;
				}

				sb.Append(c);
			}
			result.Add(sb.ToString());
			return result;
		}
	}
}
