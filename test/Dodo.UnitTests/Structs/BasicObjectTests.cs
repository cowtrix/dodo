using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Resources;
using SharedTest;
using System;
using System.Linq;

namespace UnitTests
{
	public abstract class BasicObjectTests<T>
	{
		protected abstract T Get();

		[TestMethod]
		public void Equals()
		{
			var first = Get();
			var second = first;
			AssertX.AreEqual(first, second, EqualityFunction);
		}

		[TestMethod]
		public void NotEquals()
		{
			var first = Get();
			var second = Get();
			AssertX.AreNotEqual(first, second, EqualityFunction);
		}

		[TestMethod]
		public void JSONCopyEquals()
		{
			var first = Get();
			var second = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(first));
			AssertX.AreEqual(first, second, EqualityFunction);
		}

		[TestMethod]
		public void CanSaveInPersistentStore()
		{
			var store = new PersistentStore<int, T>("test", $"_{GetType()}");
			var val = Get();
			store[0] = val;
			AssertX.AreEqual(val, store[0], EqualityFunction);
		}

		protected virtual Func<T, T, bool> EqualityFunction => null;
	}
}
