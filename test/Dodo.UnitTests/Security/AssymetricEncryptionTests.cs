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
	public class AsymmetricEncryptionTests : TestBase
	{
		[TestMethod]
		public void CanEncryptAndDecryptString()
		{
			var data = KeyGenerator.GetUniqueKey(64);
			CanEncryptAndDecrypt<string>(data);
			CanEncryptAndDecryptAsymmEncryptedStore(data);
		}

		[TestMethod]
		public void CanEncryptAndDecryptLong()
		{
			var data = 6536516874321;
			CanEncryptAndDecrypt<long>(data);
			CanEncryptAndDecryptAsymmEncryptedStore(data);
		}

		[TestMethod]
		public void CanEncryptAndDecryptInt()
		{
			var data = 1234;
			CanEncryptAndDecrypt<int>(data);
			CanEncryptAndDecryptAsymmEncryptedStore(data);
		}

		[TestMethod]
		public void CanEncryptAndDecryptObject()
		{
			var data = (42, 67, 32);
			CanEncryptAndDecrypt(data);
			CanEncryptAndDecryptAsymmEncryptedStore(data);
		}

		//[TestMethod]
		public void CanEncryptAndDecryptResourceReference()
		{
			// The resource reference is now too big to assym encrypt
			var rebellion = new ResourceReference<Rebellion>(CreateObject<Rebellion>());
			CanEncryptAndDecrypt(rebellion);
			CanEncryptAndDecryptAsymmEncryptedStore(rebellion);

			var gp = new ResourceReference<GroupResource>(CreateObject<WorkingGroup>());
			CanEncryptAndDecrypt(gp);
			CanEncryptAndDecryptAsymmEncryptedStore(gp);
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
