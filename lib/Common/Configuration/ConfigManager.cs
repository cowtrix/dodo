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
		private static string m_configPath => Path.GetFullPath($"{System.AppDomain.CurrentDomain.FriendlyName}_config.json");
		static Dictionary<string, object> m_data = new Dictionary<string, object>();

		static ConfigManager()
		{
			LoadFromFile();
		}

		public static void LoadFromFile()
		{
			if (!File.Exists(m_configPath))
			{
				Logger.Warning($"No config file found at {Path.GetFullPath(m_configPath)}");
				return;
			}
			m_data = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(m_configPath));
			Logger.Debug($"Loaded configuration data from {m_configPath}");
		}

		public static void SaveToFile()
		{
			File.WriteAllText(m_configPath, JsonConvert.SerializeObject(m_data, Formatting.Indented));
			Logger.Debug($"Saved configuration data to {m_configPath}");
		}

		public static T GetValue<T>(string key, T defaultValue)
		{
			if(!TryGetValue<T>(key, out var result))
			{
				return defaultValue;
			}
			return result;
		}

		public static bool TryGetValue<T>(string key, out T result)
		{
			if (string.IsNullOrEmpty(key))
			{
				throw new ArgumentNullException("ConfigVariable had null key");
			}
			if (!m_data.TryGetValue(key, out var obj))
			{
				result = default(T);
				return false;
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

		internal static bool SetValue<T>(string configKey, T newVal)
		{
			m_data[configKey] = newVal;
			SaveToFile();
			return true;
		}

#if DEBUG
		const string m_sampleConfigPath = @"config.sample.json";
		static Dictionary<string, object> m_sampleData = new Dictionary<string, object>();
		internal static void Register<T>(ConfigVariable<T> configVariable)
		{
			try
			{
				m_sampleData[configVariable.ConfigKey] = configVariable.DefaultValue;
				File.WriteAllText(m_sampleConfigPath, JsonConvert.SerializeObject(m_sampleData, Formatting.Indented));
			}
			catch(Exception e)
			{
				Logger.Exception(e);
			}
		}
#endif
	}
}
