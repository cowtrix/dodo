using Common.Extensions;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Users.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Resources;
using Resources.Security;
using SharedTest;

namespace Dodo.UnitTests
{
	[TestClass]
	public class UserTests : TestBase
	{
		[TestMethod]
		public void CanSerializeTokens()
		{
			var user = GetRandomUser(out var password, out var context, verifyEmail:false);
			using (var rscLock = new ResourceLock(user))
			{
				user.TokenCollection.Add(user, new ResourceCreationToken(typeof(Rebellion)));
				user.TokenCollection.Add(user, new AddAdminToken(CreateObject<LocalGroup>(), new Passphrase("1234"), user.AuthData.PublicKey));
				user.TokenCollection.Add(user, new ResetPasswordToken(user));
				user.TokenCollection.Add(user, new TemporaryUserToken(new Passphrase("1234"), "1234"));
				user.TokenCollection.Add(user, new SessionToken(user, "1234", new Passphrase("1234")));
				UserManager.Update(user, rscLock);
			}
			var userInDB = UserManager.GetSingle(u => u.Guid == user.Guid);
			Assert.AreEqual(user.TokenCollection.Count, userInDB.TokenCollection.Count);
			foreach(var token in user.TokenCollection.GetAllTokens(context))
			{
				var tokenInDB = userInDB.TokenCollection.GetToken(context, token.Guid);
				Assert.IsNotNull(tokenInDB);
				Assert.AreEqual(token.Guid, tokenInDB.Guid);
				var ogJson = JsonConvert.SerializeObject(token, JsonExtensions.StorageSettings);
				var deserializedJson = JsonConvert.SerializeObject(tokenInDB, JsonExtensions.StorageSettings);
				Assert.IsTrue(ogJson == deserializedJson,
					$"Unmatched:\n{ogJson}\n{deserializedJson}");
			}
		}
	}

}
