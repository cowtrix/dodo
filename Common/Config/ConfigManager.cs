using Newtonsoft.Json;
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
		static Dictionary<string, object> m_data = new Dictionary<string, object>();

		static ConfigManager()
		{
			const string configPath = "config.json";
			if(!File.Exists(configPath))
			{
				Logger.Warning($"No config file found at {Path.GetFullPath(configPath)}");
				return;
			}
			m_data = JsonConvert.DeserializeObject<Dictionary<string, object>>(File.ReadAllText(configPath));
		}

		internal static bool GetValue<T>(ConfigVariable<T> configVariable, out T result)
		{
			if (!m_data.TryGetValue(configVariable.ConfigKey, out var obj))
			{
				result = default;
				return false;
			}
			result = (T)obj;
			return true;
		}
	}
}
