using System;
using Common;
using Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using Resources.Security;
using Ryadel.Components.Security;

namespace Security
{
	[TestClass]
	public class SymmetricEncryptionTests
	{
		[TestMethod]
		public void CanEncryptAndDecryptString()
		{
			var data = KeyGenerator.GetUniqueKey(64);
			CanEncryptAndDecrypt<string>(data);
		}

		[TestMethod]
		public void CanEncryptAndDecryptLong()
		{
			var data = 6536516874321;
			CanEncryptAndDecrypt<long>(data);
		}

		[TestMethod]
		public void CanEncryptAndDecryptInt()
		{
			var data = 1234;
			CanEncryptAndDecrypt<int>(data);
		}

		[TestMethod]
		public void CanEncryptAndDecryptObject()
		{
			var data = new GeoLocation(42, 67);
			CanEncryptAndDecrypt<GeoLocation>(data);
		}

		private void CanEncryptAndDecrypt<T>(T data)
		{
			var pass = new Passphrase(PasswordGenerator.Generate());
			var encrypted = new EncryptedStore<T>(data, pass);
			var unencrypted = encrypted.GetValue(pass);
			Assert.AreEqual(data, unencrypted);
		}

		[TestMethod]
		public void CanCreatePassphrase()
		{
			var password = PasswordGenerator.Generate();
			var passphrase = new Passphrase(password);
			Assert.AreEqual(password, passphrase.Value);
		}
	}
}
