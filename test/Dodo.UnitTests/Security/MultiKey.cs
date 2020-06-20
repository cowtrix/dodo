using Common.Security;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources.Security;

namespace Security
{
	[TestClass]
	[TestCategory("Security")]
	public class MultiKey : TestDataBase
	{
		[TestMethod]
		public void CanCreateAndAccesString()
		{
			var firstUser = "test 1";
			var firstPass = new Passphrase(KeyGenerator.GetUniqueKey(64));
			CancCreateAndAccess<string>(firstUser, firstPass);
		}

		[TestMethod]
		public void CanAddUserString()
		{
			var firstUser = "test 1";
			var firstPass = new Passphrase(KeyGenerator.GetUniqueKey(64));
			var keystore = CancCreateAndAccess<string>(firstUser, firstPass);
			var secondUser = "test 2";
			var secondPass = new Passphrase(KeyGenerator.GetUniqueKey(64));
			keystore.Add(secondUser, secondPass);
			Assert.IsTrue(keystore.IsAuthorised(secondUser, secondPass));
			Assert.IsFalse(keystore.IsAuthorised("invalid", new Passphrase("invalid")));
		}

		public MultiSigKeyStore<T> CancCreateAndAccess<T>(T creator, Passphrase passphrase)
		{
			var multisig = new MultiSigKeyStore<T>();
			multisig.Add(creator, passphrase);
			Assert.IsTrue(multisig.IsAuthorised(creator, passphrase));
			return multisig;
		}
	}
}
