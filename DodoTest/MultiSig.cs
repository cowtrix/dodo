using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Common;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleHttpServer.REST;

namespace Security
{
	[TestClass]
	public class MultiSig
	{
		class TestEncryptedData
		{
			public class InnerClass
			{
				public double DoubleProperty { get; set; }
			}
			public string StringValue = "This is a test string";
			public string StringProperty { get; set; }
			public int IntValue = 12345;
			public GeoLocation Location = new GeoLocation(43, 62);
			public ResourceReference<User> UserReference = new ResourceReference<User>(Guid.NewGuid());
			public EncryptedStore<string> EncryptedString;
			public MultiSigEncryptedStore<string, InnerClass> EncryptedObject;
		}

		[TestMethod]
		public void CanPatch()
		{
			var key = "user";
			var password = "password";
			var data = new TestEncryptedData()
			{
				EncryptedString = new EncryptedStore<string>("my encrypted value", password),
				EncryptedObject = new MultiSigEncryptedStore<string, TestEncryptedData.InnerClass>(
					new TestEncryptedData.InnerClass()
					{
						DoubleProperty = 4.7
					}, key, password)
			};
			
			var multiSig = new MultiSigEncryptedStore<string, TestEncryptedData>(data, key, password);
			multiSig = multiSig.PatchObject(new Dictionary<string, object>()
			{
				{ "IntValue", 54321 },
				{ "StringProperty", "test" },
				{ "EncryptedString", "a new value" },
				{ "EncryptedObject", new Dictionary<string,object>()
				{
					{ "DoubleProperty", 7.1 }
				} }
			}, key, password);
			data = multiSig.GetValue(key, password);
			Assert.AreEqual(54321, data.IntValue);
			Assert.AreEqual("test", data.StringProperty);
			Assert.AreEqual("a new value", data.EncryptedString.GetValue(password));
			Assert.AreEqual(7.1, data.EncryptedObject.GetValue(key, password).DoubleProperty);
		}

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
