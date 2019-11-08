using System;
using System.Security.Cryptography;
using Common;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Security
{
	[TestClass]
	public class MultiSig
	{
		[TestMethod]
		public void IntData()
		{
			DoMultiSigTest(12345, 54321, Guid.NewGuid(), Guid.NewGuid());
		}

		[TestMethod]
		public void StringData()
		{
			DoMultiSigTest("Hello there", "obi wan", Guid.NewGuid(), Guid.NewGuid());
		}

		[TestMethod]
		public void DateTimeData()
		{
			DoMultiSigTest(DateTime.Now, DateTime.MaxValue, Guid.NewGuid(), Guid.NewGuid());
		}

		[TestMethod]
		public void ObjectData()
		{
			DoMultiSigTest(new { TestData = "this is some test data", Location = new GeoLocation(42, 51) },
				new { TestData = "this test data is different", Location = new GeoLocation(12, 99) },
				Guid.NewGuid(), Guid.NewGuid());
		}

		public void DoMultiSigTest<TKey, TVal>(TVal firstData, TVal secondData, TKey firstKey, TKey secondKey)
		{
			var firstPassword = Guid.NewGuid().ToString();
			var multisig = new MultiSigEncryptedStore<TKey, TVal>(firstData, firstKey, firstPassword);
			Assert.AreEqual(multisig.GetValue(firstKey, firstPassword), firstData);

			var secondPassword = Guid.NewGuid().ToString();
			multisig.AddPermission(firstKey, firstPassword, secondKey, secondPassword);
			Assert.AreEqual(firstData, multisig.GetValue(secondKey, secondPassword));

			// Make sure it breaks when it should
			Assert.ThrowsException<CryptographicException>(() => multisig.GetValue(firstKey, secondPassword));
			Assert.ThrowsException<CryptographicException>(() => multisig.GetValue(secondKey, "notthepassword"));
			Assert.ThrowsException<Exception>(() => multisig.GetValue(default, firstPassword));

			multisig.SetValue(secondData, secondKey, secondPassword);
			Assert.AreEqual(secondData, multisig.GetValue(firstKey, firstPassword));
			Assert.AreEqual(secondData, multisig.GetValue(secondKey, secondPassword));
		}
	}
}
