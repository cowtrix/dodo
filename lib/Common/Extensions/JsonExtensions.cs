using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common.Extensions
{
	public static class JsonExtensions
	{
		public static JsonSerializerSettings NetworkSettings
		{
			get
			{
				var settings = new JsonSerializerSettings()
				{
					CheckAdditionalContent = true,
					DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
					DateParseHandling = DateParseHandling.DateTimeOffset,
					Formatting = Formatting.Indented,
				};
				settings.Converters.Add(new StringEnumConverter());
				return settings;
			}
		}

		public static JsonSerializerSettings StorageSettings
		{
			get
			{
				var settings = new JsonSerializerSettings()
				{
					TypeNameHandling = TypeNameHandling.Objects,
					DateTimeZoneHandling = DateTimeZoneHandling.RoundtripKind,
					DateParseHandling = DateParseHandling.DateTimeOffset,
				};
				return settings;
			}
		}

		/// <summary>
		/// Verify the syntactic integrity of a JSON string
		/// </summary>
		/// <param name="value"></param>
		/// <returns></returns>
		public static bool IsValidJson(this string value)
		{
			try
			{
				var json = JContainer.Parse(value);
				return true;
			}
			catch
			{
				return false;
			}
		}

		public static T DeserializeAnonymousType<T>(string json, T anonymousObj) where T : class
		{
			var newObj = JsonConvert.DeserializeAnonymousType(json, anonymousObj);
			if (newObj == null)
			{
				throw new NullReferenceException();
			}
			return newObj;
		}

		public static string PrettifyJSON(string json)
		{
			if(string.IsNullOrEmpty(json) || !IsValidJson(json))
			{
				return json;
			}
			return JValue.Parse(json).ToString(Formatting.Indented);
		}
	}
}
