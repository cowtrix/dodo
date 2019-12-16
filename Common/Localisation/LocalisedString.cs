using System;
using System.Collections.Generic;

namespace Common.Localisation
{
	public class LocalisedString
	{
		public string Key { get; private set; }
		public Dictionary<string, string> Translations { get; private set; }

		public LocalisedString(string key)
		{
			Translations = new Dictionary<string, string>();
			Key = key;
		}

		public string GetValue(string languageCode)
		{
			Translations.TryGetValue(languageCode, out var translation);
			return translation;
		}

		internal void AddTranslation(string languageCode, string value)
		{
			Translations[languageCode] = value;
		}
	}
}
