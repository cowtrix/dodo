using System;
using System.Collections.Generic;

namespace Common
{
	public struct ConfigVariable<T>
	{
		public T Value
		{
			get
			{
				if(ConfigManager.GetValue(this, out var obj))
					return obj;
				return m_default;
			}
		}
		private T m_default;
		public string ConfigKey { get; private set; }
		public ConfigVariable(string key, T defaultValue)
		{
			m_default = defaultValue;
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
