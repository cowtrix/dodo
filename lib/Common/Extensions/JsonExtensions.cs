using Newtonsoft.Json;
using Newtonsoft.Json.Converters;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Common.Extensions
{

	public class ReferenceEqualityComparer : EqualityComparer<Object>
	{
		public override bool Equals(object x, object y)
		{
			return ReferenceEquals(x, y);
		}
		public override int GetHashCode(object obj)
		{
			if (obj == null) return 0;
			return obj.GetHashCode();
		}
	}

	namespace ArrayExtensions
	{
		public static class ArrayExtensions
		{
			public static void ForEach(this Array array, Action<Array, int[]> action)
			{
				if (array.LongLength == 0) return;
				ArrayTraverse walker = new ArrayTraverse(array);
				do action(array, walker.Position);
				while (walker.Step());
			}
		}

		internal class ArrayTraverse
		{
			public int[] Position;
			private int[] maxLengths;

			public ArrayTraverse(Array array)
			{
				maxLengths = new int[array.Rank];
				for (int i = 0; i < array.Rank; ++i)
				{
					maxLengths[i] = array.GetLength(i) - 1;
				}
				Position = new int[array.Rank];
			}

			public bool Step()
			{
				for (int i = 0; i < Position.Length; ++i)
				{
					if (Position[i] < maxLengths[i])
					{
						Position[i]++;
						for (int j = 0; j < i; j++)
						{
							Position[j] = 0;
						}
						return true;
					}
				}
				return false;
			}
		}
	}

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
