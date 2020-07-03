using System;
using Common;
using Common.Security;
using Dodo;
using Dodo.Rebellions;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using Resources.Security;
using SharedTest;

namespace Security
{
	[TestClass]
	[TestCategory("Security")]
	public class AsymmetricEncryptionTests : TestBase
	{
		[TestMethod]
		public void CanEncryptAndDecryptString()
		{
			var data = KeyGenerator.GetUniqueKey(64);
			CanEncryptAndDecrypt<string>(data);
			CanEncryptAndDecryptAsymmEncryptedStore(data);
			CanSetValue(data);
			CanTryGetValue(data);
		}

		[TestMethod]
		public void CanEncryptAndDecryptLong()
		{
			var data = 6536516874321;
			CanEncryptAndDecrypt<long>(data);
			CanEncryptAndDecryptAsymmEncryptedStore(data);
			CanSetValue(data);
			CanTryGetValue(data);
		}

		[TestMethod]
		public void CanEncryptAndDecryptInt()
		{
			var data = 1234;
			CanEncryptAndDecrypt<int>(data);
			CanEncryptAndDecryptAsymmEncryptedStore(data);
			CanSetValue(data);
			CanTryGetValue(data);
		}

		[TestMethod]
		public void CanEncryptAndDecryptObject()
		{
			var data = (42, 67, 32);
			CanEncryptAndDecrypt(data);
			CanEncryptAndDecryptAsymmEncryptedStore(data);
			CanSetValue(data);
			CanTryGetValue(data);
		}

		private void CanTryGetValue<T>(T data)
		{
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			var store = new AsymmEncryptedStore<T>(data, new Passphrase(pk));
			Assert.IsTrue(store.TryGetValue(null, new Passphrase(pv), out var dataObj));
			Assert.AreEqual(data, dataObj);
		}

		private void CanSetValue<T>(T data)
		{
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			var store = new AsymmEncryptedStore<T>();
			store.SetValue(data, new Passphrase(pk));
			var decryptedData = store.GetValue(new Passphrase(pv));
			Assert.AreEqual(data, decryptedData);
		}

		private void CanEncryptAndDecrypt<T>(T data)
		{
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			var encryptedData = AsymmetricSecurity.Encrypt(data, pk);
			var decryptedData = AsymmetricSecurity.Decrypt<T>(encryptedData, pv);
			Assert.AreEqual(data, decryptedData);
		}

		private void CanEncryptAndDecryptAsymmEncryptedStore<T>(T data)
		{
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			var assym = new AsymmEncryptedStore<T>(data, new Passphrase(pk));
			var decryptedData = assym.GetValue(pv);
			Assert.AreEqual(data, decryptedData);
		}
	}
}
