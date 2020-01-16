using System;
using Common;
using Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Security
{
	[TestClass]
	public class AsymmetricEncryptionTests
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
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			var encryptedData = AsymmetricSecurity.Encrypt(data, pk);
			var decryptedData = AsymmetricSecurity.Decrypt<T>(encryptedData, pv);
			Assert.AreEqual(data, decryptedData);
		}
	}
}
