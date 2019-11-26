using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;

namespace Common
{
	public static class ConfigManager
	{
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

		static Dictionary<string, object> m_data = new Dictionary<string, object>();

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
