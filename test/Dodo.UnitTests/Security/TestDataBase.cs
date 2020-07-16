using System;
using Common;
using Common.Extensions;
using Common.Security;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using Resources.Location;
using Resources.Security;

namespace Security
{
	public abstract class TestDataBase
	{
		protected class TestEncryptedData
		{
			public class InnerClass
			{
				[View(EPermissionLevel.USER, EPermissionLevel.USER)]
				public double DoubleProperty { get; set; }
			}

			public string StringValue = "This is a test string";
			[View(EPermissionLevel.USER, EPermissionLevel.USER)]
			public string StringProperty { get; set; }
			[View(EPermissionLevel.USER, EPermissionLevel.USER)]
			public int IntValue = 12345;
			[View(EPermissionLevel.OWNER)]
			public GeoLocation Location = new GeoLocation(43, 62);
			public ResourceReference<User> UserReference = 
				new ResourceReference<User>(Guid.NewGuid(), "asdadw", typeof(User), "asdadw", null, default, default);
			[View(EPermissionLevel.USER, EPermissionLevel.USER)]
			public SymmEncryptedStore<string> EncryptedString;
			[View(EPermissionLevel.USER, EPermissionLevel.USER)]
			public MultiSigEncryptedStore<string, InnerClass> EncryptedObject;
			[View(EPermissionLevel.USER, EPermissionLevel.USER)]
			public MultiSigEncryptedStore<string, GeoLocation> EncryptedObjectProp { get; set; }
		}
	}
}
