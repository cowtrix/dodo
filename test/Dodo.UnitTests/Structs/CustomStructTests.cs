using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;

namespace Dodo.UnitTests
{
	public abstract class CustomStructTests<T> where T:struct
	{
		protected abstract T Get();

		[TestMethod]
		public void Equals()
		{
			var first = Get();
			var second = first;
			Assert.IsTrue(first.Equals(second));
		}

		[TestMethod]
		public void NotEquals()
		{
			var first = Get();
			var second = Get();
			Assert.IsFalse(first.Equals(second));
		}

		[TestMethod]
		public void JSONCopyEquals()
		{
			var first = Get();
			var second = JsonConvert.DeserializeObject<T>(JsonConvert.SerializeObject(first));
			Assert.IsTrue(first.Equals(second));
		}
	}
}
