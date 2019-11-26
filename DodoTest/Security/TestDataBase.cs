using System;
using Common;
using Common.Security;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using SimpleHttpServer.REST;

namespace Security
{
	public abstract class TestDataBase
	{
		protected class TestEncryptedData
		{
			public class InnerClass
			{
				[View(EUserPriviligeLevel.USER, EUserPriviligeLevel.USER)]
				public double DoubleProperty { get; set; }
			}

			public string StringValue = "This is a test string";
			[View(EUserPriviligeLevel.USER, EUserPriviligeLevel.USER)]
			public string StringProperty { get; set; }
			[View(EUserPriviligeLevel.USER, EUserPriviligeLevel.USER)]
			public int IntValue = 12345;
			[View(EUserPriviligeLevel.OWNER)]
			public GeoLocation Location = new GeoLocation(43, 62);
			public ResourceReference<User> UserReference = new ResourceReference<User>(Guid.NewGuid());
			[View(EUserPriviligeLevel.USER, EUserPriviligeLevel.USER)]
			public EncryptedStore<string> EncryptedString;
			[View(EUserPriviligeLevel.USER, EUserPriviligeLevel.USER)]
			public MultiSigEncryptedStore<string, InnerClass> EncryptedObject;
			[View(EUserPriviligeLevel.USER, EUserPriviligeLevel.USER)]
			public MultiSigEncryptedStore<string, GeoLocation> EncryptedObjectProp { get; set; }
		}
	}
}
