using System;
using Common.Extensions;
using DodoTest.Framework;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace DodoTest.Serialization
{
	[TestClass]
	public abstract class SerializationTestBase<T>
	{
		[TestMethod]
		public void CanSerializeAndDeserialize()
		{
			T obj = GetObject();
			var json = JsonConvert.SerializeObject(obj, JsonExtensions.DefaultSettings);
			var deserializedObj = JsonConvert.DeserializeObject<T>(json);
			Assert.IsTrue(Compare(obj, deserializedObj));
		}

		protected abstract T GetObject();

		protected virtual bool Compare(T first, T second)
		{
			return CompareHelper.CompareObjects(first, second, null);
		}
	}
}
