using Common.Extensions;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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
		const string m_configPath = "config.json";
		static Dictionary<string, object> m_data = new Dictionary<string, object>();

		static ConfigManager()
		{
			LoadFromFile();
		}

		[Command("^config load$", "config load", "Load configuration from file")]
		public static void LoadFromFile(string args = null)
		{
			if (!File.Exists(m_configPath))
			{
				Logger.Warning($"No config file found at {Path.GetFullPath(m_configPath)}");
				return;
			}
			m_data = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(m_configPath), JsonExtensions.DatabaseSettings);
			Logger.Debug($"Loaded configuration data from {m_configPath}");
		}

		[Command("^config save", "config save", "Save configuration to file")]
		public static void SaveToFile(string args = null)
		{
			File.WriteAllText(m_configPath, JsonConvert.SerializeObject(m_data, JsonExtensions.DatabaseSettings));
			Logger.Debug($"Saved configuration data to {m_configPath}");
		}

		internal static bool GetValue<T>(ConfigVariable<T> configVariable, out T result)
		{
			if (!m_data.TryGetValue(configVariable.ConfigKey, out var obj))
			{
				result = default;
				return false;
			}
			result = (T)Convert.ChangeType(obj, typeof(T));
			return true;
		}

#if DEBUG
		const string m_sampleConfigPath = @"..\..\config.sample.json";
		static Dictionary<string, object> m_sampleData = new Dictionary<string, object>();
		internal static void Register<T>(ConfigVariable<T> configVariable)
		{
			//m_sampleData[configVariable.ConfigKey] = configVariable.DefaultValue;
			//File.WriteAllText(m_sampleConfigPath, JsonConvert.SerializeObject(m_sampleData, JsonExtensions.DatabaseSettings));
		}
#endif
	}
}
