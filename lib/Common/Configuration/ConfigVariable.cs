using Newtonsoft.Json;
using System;
using System.Collections.Generic;

namespace Common.Config
{
	/// <summary>
	/// This is a variable that can be overriden in the configuration JSON file.
	/// Each ConfigurationVariable has a string key. Non-default values can be
	/// specified in a `config.json` file in the root directory, which should
	/// be formatted as a JSON string/object dictionary
	/// </summary>
	/// <typeparam name="T">The type of the value to be stored</typeparam>
	public struct ConfigVariable<T>
	{
		public T Value
		{
			get
			{
				if(ConfigManager.TryGetValue(this, out var obj))
					return obj;
				if (m_required)
					throw new Exception($"{ConfigManager.ConfigPath}\nConfig variable '{ConfigKey}' is required\n{JsonConvert.SerializeObject(default(T), Formatting.Indented)}");
				return DefaultValue;
			}
			set
			{
				ConfigManager.SetValue(ConfigKey, value);
			}
		}
		private bool m_required { get; set; }
		public T DefaultValue { get; private set; }
		public string ConfigKey { get; private set; }
		public ConfigVariable(string key, T defaultValue)
		{
			DefaultValue = defaultValue;
			ConfigKey = key;
			m_required = false;
		}
		public ConfigVariable(string key)
		{
			DefaultValue = default;
			m_required = true;
			ConfigKey = key;
		}

		public override bool Equals(object obj)
		{
			if (!(obj is ConfigVariable<T>))
			{
				return false;
			}

			var variable = (ConfigVariable<T>)obj;
			return ConfigKey == variable.ConfigKey;
		}

		public override int GetHashCode()
		{
			return 461768814 + EqualityComparer<string>.Default.GetHashCode(ConfigKey);
		}
	}
}
