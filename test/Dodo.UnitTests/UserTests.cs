using Common.Extensions;
using Dodo;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Users;
using Dodo.Users.Tokens;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Resources;
using Resources.Security;
using SharedTest;
using System;
using System.Linq;

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
				user.TokenCollection.AddOrUpdate(user, new ResourceCreationToken(typeof(Rebellion)));
				user.TokenCollection.AddOrUpdate(user, new UserAddedAsAdminToken(CreateObject<WorkingGroup>().CreateRef<IAdministratedResource>(), 
					new Passphrase("1234"), user.AuthData.PublicKey, user));
				user.TokenCollection.AddOrUpdate(user, new ResetPasswordToken(user));
				user.TokenCollection.AddOrUpdate(user, new TemporaryUserToken(new Passphrase("1234"), "1234"));
				user.TokenCollection.AddOrUpdate(user, new SessionToken(user, "1234", new Passphrase("1234"), new System.Net.IPAddress(0)));
				UserManager.Update(user, rscLock);
			}
			var userInDB = UserManager.GetSingle(u => u.Guid == user.Guid);
			Assert.AreEqual(user.TokenCollection.Count, userInDB.TokenCollection.Count);
			foreach(var token in user.TokenCollection.GetAllTokens(context, EPermissionLevel.SYSTEM, user))
			{
				var tokenInDB = userInDB.TokenCollection.GetToken(context, token.Guid);
				Assert.IsNotNull(tokenInDB);
				Assert.AreEqual(token.Guid, tokenInDB.Guid);
				var ogJson = JsonConvert.SerializeObject(token, JsonExtensions.StorageSettings);
				var deserializedJson = JsonConvert.SerializeObject(tokenInDB, JsonExtensions.StorageSettings);
				Assert.IsTrue(ogJson == deserializedJson,
					$"Unmatched:\n{ogJson}\n{deserializedJson}");
			}
			var adminToken = userInDB.TokenCollection.GetAllTokens<UserAddedAsAdminToken>(context, EPermissionLevel.OWNER, user).Single();
			Assert.IsTrue(adminToken.Reference.Parent != default);
		}

		[TestMethod]
		public void CannotDuplicateUsername()
		{
			var user1 = GetRandomUser(out _, out _);
			AssertX.Throws<Exception>(() =>
				GenerateUser(new UserSchema("second", user1.Slug, "[@ssw0rd", "test@web.com"), out _),
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
