using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Common
{
	public static class JsonExtensions
	{
		public static T DeserializeAnonymousType<T>(string json, T anonymousObj) where T : class
		{
			try
			{
				var newObj = JsonConvert.DeserializeAnonymousType(json, anonymousObj);
				if(newObj == null)
				{
					throw new NullReferenceException();
				}
				return newObj;
			}
			catch(Exception e)
			{
				throw new Exception($"Failed to deserialise JSON. Expected:\n {JsonConvert.SerializeObject(anonymousObj, Formatting.Indented)}");
			}
		}
	}
}
