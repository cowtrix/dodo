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
		private static string m_configPath => Path.Combine(
			Path.GetDirectoryName(Process.GetCurrentProcess().MainModule.FileName), $"{System.AppDomain.CurrentDomain.FriendlyName}_config.json");
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

		internal static bool GetValue<T>(ConfigVariable<T> configVariable, out T result)
		{
			if (!m_data.TryGetValue(configVariable.ConfigKey, out var obj))
			{
				result = default(T);
				return false;
			}
			if(typeof(Enum).IsAssignableFrom(typeof(T)))
			{
				result = (T)Enum.Parse(typeof(T), obj.ToString());
			}
			else
			{
				result = (T)Convert.ChangeType(obj, typeof(T));
			}
			return true;
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
			m_sampleData[configVariable.ConfigKey] = configVariable.DefaultValue;
			File.WriteAllText(m_sampleConfigPath, JsonConvert.SerializeObject(m_sampleData, Formatting.Indented));
		}
#endif
	}
}
