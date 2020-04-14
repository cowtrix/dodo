using Common;
using Common.Extensions;
using Dodo;
using Dodo.LocalGroups;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources;
using System;
using SharedTest;
using System.Threading.Tasks;
using Dodo.SharedTest;
using Dodo.Users.Tokens;
using DodoTest.Framework.Postman;

namespace RESTTests
{
	[TestClass]
	public class UserTests : IntegrationTestBase
	{
		const string UserCat = "Users";

		[AssemblyInitialize]
		public static void SetupTests(TestContext testContext)
		{
			ResourceUtility.ClearAllManagers();
		}

		[TestMethod]
		public async Task CanRegisterNewUser()
		{
			var response = await Request($"{UserController.RootURL}/{UserController.REGISTER}", EHTTPRequestType.POST, SchemaGenerator.GetRandomUser());
			Assert.IsTrue(response.IsSuccessStatusCode, response.ToString());
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Register a new user" },
				LastRequest);
		}

		[TestMethod]
		public async Task CanUpdateProfile()
		{
			var user = GetRandomUser(out var password, out var context);
			var lg = new LocalGroupFactory().CreateObject(context, SchemaGenerator.GetRandomLocalGroup(context));
			await Login(user.AuthData.Username, password);
			const string newName = "My New Name";
			var patchObj = new
			{
				Name = newName,
				PersonalData = new
				{
					LocalGroup = new 
					{
						Guid = lg.Guid
					}
				}
			};
			var response = await Request($"{UserController.RootURL}/{user.Guid}", EHTTPRequestType.PATCH, patchObj);
			Assert.IsTrue(response.IsSuccessStatusCode);
			user = ResourceUtility.GetManager<User>().GetSingle(x => x.Guid == user.Guid);
			Assert.AreEqual(newName, user.Name);
			Assert.AreEqual(user.PersonalData.LocalGroup.Guid, lg.Guid);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Update a User" },
				LastRequest);
		}

		[TestMethod]
		public async Task CanLogin()
		{
			var user = GetRandomUser(out var password, out _);
			await Login(user.AuthData.Username, password);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Login" },
				LastRequest);
			var response = await Request($"{UserController.RootURL}/{user.Guid}", EHTTPRequestType.GET);
			Assert.IsTrue(response.IsSuccessStatusCode, response.ToString());
		}

		[TestMethod]
		public async Task CanLogout()
		{
			var user = GetRandomUser(out var password, out _);
			await Login(user.AuthData.Username, password);
			await Request($"{UserController.RootURL}/{user.Guid}", EHTTPRequestType.GET);
			await Logout();
			await AssertX.ThrowsAsync<Exception>(Request($"{UserController.RootURL}/{user.Guid}", EHTTPRequestType.GET),
				e => e.Message.Contains("StatusCode: 302, ReasonPhrase: 'Found'"));
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Logout" },
				LastRequest);
		}

		[TestMethod]
		public async Task CanDeleteProfile()
		{
			var user = GetRandomUser(out var password, out _);
			await Login(user.AuthData.Username, password);
			await Request($"{UserController.RootURL}/{user.Guid}", EHTTPRequestType.DELETE);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Delete a User" },
				LastRequest);
		}

		[TestMethod]
		public async Task CannotLoginWithBadAuth()
		{
			var user = GetRandomUser(out var password, out _);
			await AssertX.ThrowsAsync<Exception>(Login(user.AuthData.Username, "not the password"),
				e => e.Message.Contains("Bad Request"));
		}

		[TestMethod]
		public async virtual Task CannotGetAnonymously()
		{
			var user = GetRandomUser(out _, out _);
			await AssertX.ThrowsAsync<Exception>(RequestJSON($"{UserController.RootURL}/{user.Guid.ToString()}", EHTTPRequestType.GET));
		}

		[TestMethod]
		public async virtual Task CanGetOwnProfile()
		{
			var user = GetRandomUser(out var password, out var context);
			await Login(user.AuthData.Username, password);
			await RequestJSON($"{UserController.RootURL}/{user.Guid.ToString()}", EHTTPRequestType.GET);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Get a User" },
				LastRequest);
		}

		[TestMethod]
		public async Task CanResetPassword()
		{
			var user = GetRandomUser(out var password, out var context);
			var request = await Request($"{UserController.RootURL}/{UserController.RESET_PASSWORD}", EHTTPRequestType.GET,
				null, new[] { ( "email", user.PersonalData.Email) } );
			var newPassword = ValidationExtensions.GenerateStrongPassword();
			user = UserManager.GetSingle(u => u.Guid == user.Guid);
			var token = user.TokenCollection.GetSingleToken<ResetPasswordToken>();

			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Request a Password Reset Token" },
				request);

			request = await Request($"{UserController.RootURL}/{UserController.RESET_PASSWORD}", EHTTPRequestType.POST,
				newPassword, new[] { ( UserController.PARAM_TOKEN, token.TemporaryToken ) },
				r => r.StatusCode == System.Net.HttpStatusCode.Redirect);

			await Login(user.AuthData.Username, newPassword);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Redeem a Password Reset Token" },
				request);
		}

		[TestMethod]
		public async Task CanChangePassword()
		{
			var user = GetRandomUser(out var password, out var context);
			var newPassword = ValidationExtensions.GenerateStrongPassword();
			await Login(user.AuthData.Username, password);
			var req = await Request($"{UserController.RootURL}/{UserController.CHANGE_PASSWORD}", EHTTPRequestType.POST, new { currentpassword = password, newpassword = newPassword });
			await Login(user.AuthData.Username, newPassword);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Change your password" },
				req);
		}

		[TestMethod]
		public async Task CanVerifyEmail()
		{
			var user = GetRandomUser(out var password, out var context, false);
			var token = user.TokenCollection.GetSingleToken<VerifyEmailToken>();
			Assert.IsNotNull(token);
			await Login(user.AuthData.Username, password);
			var request = await Request($"{UserController.RootURL}/{UserController.VERIFY_EMAIL}", EHTTPRequestType.GET,
				null, new[] { ("token", token.Token) },
				r => r.StatusCode == System.Net.HttpStatusCode.Redirect);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Redeem an Email Verification Token" },
				LastRequest);
		}
	}
}
