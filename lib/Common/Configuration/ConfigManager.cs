using Common.Commands;
using Common.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;

namespace Common.Config
{
	/// <summary>
	/// This class will save and load configuration variables, which are stored by a string key.
	/// To access a configuration variable, declare a ConfigurationVariable<T> for a given key.
	/// To set a configuration variable, add it to the "config.json" file.
	/// </summary>
	public static class ConfigManager
	{
		// TODO: this is the wrong path if hosting in IIS (maybe use IWebHostEnvironment to get it)
		public static string ConfigPath = Path.GetFullPath($"{System.AppDomain.CurrentDomain.FriendlyName}_config.json");
		static Dictionary<string, object> m_data = new Dictionary<string, object>();

		public static void LoadFromFile()
		{
			if (!File.Exists(ConfigPath))
			{
				Logger.Warning($"No config file found at {Path.GetFullPath(ConfigPath)}");
				return;
			}
			m_data = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(ConfigPath));
			Logger.Debug($"Loaded configuration data from {ConfigPath}");
		}

		public static T GetValue<T>(string key, T defaultValue)
		{
			if(!TryGetValue<T>(key, out var result))
			{
				return defaultValue;
			}
			return result;
		}

		public static void SetValue<T>(string key, T value)
		{
			m_data[key] = value;
		}

		public static bool TryGetValue<T>(string key, out T result)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("ConfigVariable had null key");
			}
			if (!m_data.TryGetValue(key, out var obj))
			{
				var env = Environment.GetEnvironmentVariable(key);
				if (!string.IsNullOrEmpty(env))
				{
					obj = env;
				}
				else
				{
					result = default(T);
					return false;
				}
			}
			if (typeof(Enum).IsAssignableFrom(typeof(T)))
			{
				result = (T)Enum.Parse(typeof(T), obj.ToString());
			}
			else if (obj is IConvertible)
			{
				result = (T)Convert.ChangeType(obj, typeof(T));
			}
			else
			{
				result = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(obj));
			}
			return true;
		}

		internal static bool TryGetValue<T>(ConfigVariable<T> configVariable, out T result)
		{
			return TryGetValue<T>(configVariable.ConfigKey, out result);
		}
	}
}
