using System;
using System.Collections.Generic;
using Common;
using Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RESTTests;
using SimpleHttpServer.REST;

namespace Security
{
	[TestClass]
	public class RawData : TestDataBase
	{
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
					}, key, password),
				EncryptedObjectProp = new MultiSigEncryptedStore<string, GeoLocation>(
					new GeoLocation(45, 45), key, password)
			};

			data.PatchObject(new Dictionary<string, object>()
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
			var password = "password";
			var data = new TestEncryptedData();
			AssertX.Throws<Exception>(() => data = data.PatchObject(new Dictionary<string, object>()
			{
				{ "StringValue", "this shouldn't succeed" },
			}, EPermissionLevel.USER, key, password), e => e.Message.Contains("Insufficient privileges"));
		}

		[TestMethod]
		public void CannotPatchWithInsufficientView()
		{
			var key = "user";
			var password = "password";
			var data = new TestEncryptedData();
			AssertX.Throws<Exception>(() => data = data.PatchObject(new Dictionary<string, object>()
			{
				{ "Location", new GeoLocation() },
			}, EPermissionLevel.USER, key, password), e => e.Message.Contains("Insufficient privileges"));
		}

		[TestMethod]
		public void NonexistantFieldThrowsError()
		{
			var key = "user";
			var password = "password";
			var data = new TestEncryptedData();
			AssertX.Throws<Exception>(() => data = data.PatchObject(new Dictionary<string, object>()
			{
				{ "NonExistantField", new GeoLocation() },
			}, EPermissionLevel.USER, key, password), e => e.Message.Contains("Invalid field names"));
		}
	}
}
