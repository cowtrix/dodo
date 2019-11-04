using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
	public static class JsonExtensions
	{
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
			if(newObj == null)
			{
				throw new NullReferenceException();
			}
			return newObj;
		}
	}
}
