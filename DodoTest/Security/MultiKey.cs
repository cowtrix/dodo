using System;
using System.Collections.Generic;
using System.Security.Cryptography;
using Common;
using Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RESTTests;
using SimpleHttpServer.REST;

namespace Security
{
	[TestClass]
	public class MultiKey : TestDataBase
	{
		[TestMethod]
		public void CanCreateAndAccesString()
		{
			var firstUser = "test 1";
			var firstPass = KeyGenerator.GetUniqueKey(64);
			CancCreateAndAccess<string>(firstUser, firstPass);
		}

		[TestMethod]
		public void CanAddUserString()
		{
			var firstUser = "test 1";
			var firstPass = KeyGenerator.GetUniqueKey(64);
			var keystore = CancCreateAndAccess<string>(firstUser, firstPass);
			var secondUser = "test 2";
			var secondPass = KeyGenerator.GetUniqueKey(64);
			keystore.AddPermission(firstUser, firstPass, secondUser, secondPass);
			Assert.IsTrue(keystore.GetValue(secondUser, secondPass));
			Assert.ThrowsException<Exception>(() => keystore.GetValue("invalid", "invalid"));
		}

		public MultiSigKeyStore<T> CancCreateAndAccess<T>(T creator, string passphrase)
		{
			var multisig = new MultiSigKeyStore<T>(creator, passphrase);
			Assert.IsTrue(multisig.GetValue(creator, passphrase));
			return multisig;
		}
	}
}
