using Common.Extensions;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Users;
using Dodo.Users.Tokens;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Resources;
using Resources.Security;
using SharedTest;
using System;

namespace UnitTests
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

		[TestMethod]
		public void CannotDuplicateUsername()
		{
			var user1 = GetRandomUser(out _, out _);
			AssertX.Throws<Exception>(() =>
				GenerateUser(new UserSchema("second", user1.AuthData.Username, "[@ssw0rd", "test@web.com"), out _),
				e => e.Message.Contains("A user with that username already exists"));
		}

		[TestMethod]
		public void CannotDuplicateEmail()
		{
			var user1 = GetRandomUser(out _, out _);
			AssertX.Throws<Exception>(() =>
				GenerateUser(new UserSchema("second", "second", "[@ssw0rd", user1.PersonalData.Email), out _),
				e => e.Message.Contains("A user with that email already exists"));
		}

		[TestMethod]
		public void CanChangePassword()
		{
			string newPassword = ValidationExtensions.GenerateStrongPassword();
			var user1 = GetRandomUser(out var password, out _);
			user1.AuthData.ChangePassword(new Passphrase(password), new Passphrase(newPassword));
			Assert.IsTrue(user1.AuthData.ChallengePassword(newPassword, out _));
		}
	}

}
