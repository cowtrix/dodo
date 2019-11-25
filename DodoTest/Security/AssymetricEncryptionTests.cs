using System;
using Common;
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
			AssymetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			var encryptedData = AssymetricSecurity.Encrypt(data, pk);
			var decryptedData = AssymetricSecurity.Decrypt<T>(encryptedData, pv);
			Assert.AreEqual(data, decryptedData);
		}
	}
}
