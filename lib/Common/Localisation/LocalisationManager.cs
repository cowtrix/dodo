using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Common.Config;
using Common.Extensions;
using Newtonsoft.Json;

namespace Common.Localisation
{
	public static class LocalisationManager
	{
		private static ConfigVariable<string> m_localisationDirectory = new ConfigVariable<string>("LocalisationManager_Directory", "localisation");
		private static Dictionary<string, LocalisedString> Phrases = new Dictionary<string, LocalisedString>();

		static LocalisationManager()
		{
			Load();
		}

		public static void Save()
		{
			var dirPath = Path.GetFullPath(m_localisationDirectory.Value);
			if (!Directory.Exists(dirPath))
			{
				Directory.CreateDirectory(dirPath);
			}
			var files = Directory.GetFiles(dirPath, "*.json");
			foreach(var translationFile in files)
			{
				File.Delete(translationFile);
			}
			var data = new Dictionary<string, Dictionary<string, string>>();
			foreach(var phrase in Phrases)
			{
				foreach (var translation in phrase.Value.Translations)
				{
					if(!data.TryGetValue(translation.Key, out var translations))
					{
						translations = new Dictionary<string, string>();
						data[translation.Key] = translations;
					}
					translations[phrase.Key] = translation.Value;
				}
			}
			foreach(var languageCode in data)
			{
				File.WriteAllText(Path.Combine(dirPath, $"{languageCode.Key}.json"),
					JsonConvert.SerializeObject(languageCode.Value, JsonExtensions.NetworkSettings));
			}
		}

		public static void Load()
		{
			var dirPath = Path.GetFullPath(m_localisationDirectory.Value);
			if(!Directory.Exists(dirPath))
			{
				Logger.Warning($"No localisation directory found at {dirPath}");
				return;
			}
			var files = Directory.GetFiles(dirPath, "*.json");
			if(!files.Any())
			{
				Logger.Warning($"No localisation files found in {dirPath}. " +
					"This is expecting .json files with their name being the country code it is for.");
				return;
			}
			Phrases = new Dictionary<string, LocalisedString>();
			foreach(var translationFile in files)
			{
				var languageCode = Path.GetFileNameWithoutExtension(translationFile);
				var phrases = JsonConvert.DeserializeObject<Dictionary<string, string>>(File.ReadAllText(translationFile),
					JsonExtensions.NetworkSettings);
				foreach(var phrase in phrases)
				{
					if (!Phrases.TryGetValue(phrase.Key, out var localisedString))
					{
						localisedString = new LocalisedString(phrase.Key);
						Phrases[phrase.Key] = localisedString;
					}
					localisedString.AddTranslation(languageCode, phrase.Value);
				}
			}
		}
	}
}
