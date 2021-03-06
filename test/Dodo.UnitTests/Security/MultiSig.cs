using System;
using System.Collections.Generic;
using System.Security.Authentication;
using Common;
using Common.Security;
using Dodo.SharedTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using Resources.Location;
using Resources.Security;
using SharedTest;

namespace Security
{
	[TestClass]
	public class MultiSig : TestDataBase
	{
		[TestMethod]
		public void CanPatch()
		{
			var key = "user";
			var password = new Passphrase("password");
			var data = new TestEncryptedData()
			{
				EncryptedString = new SymmEncryptedStore<string>("my encrypted value", password),
				EncryptedObject = new MultiSigEncryptedStore<string, TestEncryptedData.InnerClass>(
					new TestEncryptedData.InnerClass()
					{
						DoubleProperty = 4.7
					}, key, password),
				EncryptedObjectProp = new MultiSigEncryptedStore<string, GeoLocation>(
					new GeoLocation(45, 45), key, password)
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
				} },
				{ "EncryptedObjectProp", new GeoLocation(4, 67) }
			}, EPermissionLevel.USER, key, password);
			data = multiSig.GetValue(key, password);
			Assert.AreEqual(54321, data.IntValue);
			Assert.AreEqual("test", data.StringProperty);
			Assert.AreEqual("a new value", data.EncryptedString.GetValue(password));
			Assert.AreEqual(7.1, data.EncryptedObject.GetValue(key, password).DoubleProperty);
			Assert.AreEqual(4, data.EncryptedObjectProp.GetValue(key, password).Latitude);
		}

		[TestMethod]
		public void CannotPatchWithoutViewAttribute()
		{
			var key = "user";
			var password = new Passphrase("password");
			var data = new TestEncryptedData();

			var multiSig = new MultiSigEncryptedStore<string, TestEncryptedData>(data, key, password);
			AssertX.Throws<Exception>(() => multiSig = multiSig.PatchObject(new Dictionary<string, object>()
			{
				{ "StringValue", "this shouldn't succeed" },
			}, EPermissionLevel.USER, key, password), e => e.Message.Contains("Insufficient privileges"));
		}

		[TestMethod]
		public void CanChangeToken()
		{
			var key = Guid.NewGuid().ToString();
			var password = new Passphrase(Guid.NewGuid().ToString());
			var data = KeyGenerator.GetUniqueKey(128);
			var multiSig = new MultiSigEncryptedStore<string, string>(data, key, password);
			var newPassword = new Passphrase(Guid.NewGuid().ToString());
			multiSig.AddPermission(key, password, key, newPassword);
			Assert.AreEqual(data, multiSig.GetValue(key, newPassword));
		}

		[TestMethod]
		public void CannotPatchWithInsufficientView()
		{
			var key = "user";
			var password = new Passphrase("password");
			var data = new TestEncryptedData();

			var multiSig = new MultiSigEncryptedStore<string, TestEncryptedData>(data, key, password);
			AssertX.Throws<Exception>(() => multiSig = multiSig.PatchObject(new Dictionary<string, object>()
			{
				{ "Location", SchemaGenerator.RandomLocation },
			}, EPermissionLevel.USER, key, password), e => e.Message.Contains("Insufficient privileges"));
		}

		[TestMethod]
		public void NonexistantFieldThrowsError()
		{
			var key = "user";
			var password = new Passphrase("password");
			var data = new TestEncryptedData();
			var multiSig = new MultiSigEncryptedStore<string, TestEncryptedData>(data, key, password);
			AssertX.Throws<Exception>(() => multiSig = multiSig.PatchObject(new Dictionary<string, object>()
			{
				{ "NonExistantField", SchemaGenerator.RandomLocation },
			}, EPermissionLevel.USER, key, password), e => e.Message.Contains("Invalid field names"));
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
			var firstPassword = new Passphrase(Guid.NewGuid().ToString());
			var multisig = new MultiSigEncryptedStore<TKey, TVal>(firstData, firstKey, firstPassword);
			Assert.AreEqual(multisig.GetValue(firstKey, firstPassword), firstData);

			var secondPassword = new Passphrase(Guid.NewGuid().ToString());
			multisig.AddPermission(firstKey, firstPassword, secondKey, secondPassword);
			Assert.AreEqual(firstData, multisig.GetValue(secondKey, secondPassword));

			// Make sure it breaks when it should
			Assert.ThrowsException<AuthenticationException>(() => multisig.GetValue(firstKey, secondPassword));
			Assert.ThrowsException<AuthenticationException>(() => multisig.GetValue(secondKey, new Passphrase("notthepassword")));
			Assert.ThrowsException<AuthenticationException>(() => multisig.GetValue(default, firstPassword));

			multisig.SetValue(secondData, secondKey, secondPassword);
			Assert.AreEqual(secondData, multisig.GetValue(firstKey, firstPassword));
			Assert.AreEqual(secondData, multisig.GetValue(secondKey, secondPassword));
		}
	}
}
