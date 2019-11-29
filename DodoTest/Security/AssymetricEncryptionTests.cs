using System;
using Common;
using Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace Security
{
	[TestClass]
	public class AssymetricEncryptionTests
	{
		[TestMethod]
		public void CanEncryptAndDecryptString()
		{
			var data = KeyGenerator.GetUniqueKey(64);
			CanEncryptAndDecrypt<string>(data);
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
