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
using System.Linq;

namespace RESTTests
{
	[TestClass]
	public class UserTests : IntegrationTestBase
	{
		const string UserCat = "Users";

		[TestMethod]
		public async Task CanRegisterNewUser()
		{
			var response = await Request($"{UserService.RootURL}/{UserService.REGISTER}", EHTTPRequestType.POST, SchemaGenerator.GetRandomUser());
			Assert.IsTrue(response.IsSuccessStatusCode, response.ToString());
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Register a new user" },
				LastRequest);
		}

		[TestMethod]
		public async Task CanUpdateProfile()
		{
			var user = GetRandomUser(out var password, out var context);
			var lg = CreateObject<LocalGroup>(context, SchemaGenerator.GetRandomLocalGroup(context));
			await Login(user.Slug, password);
			const string newName = "My New Name";
			var patchObj = new
			{
				Name = newName,
				PersonalData = new
				{
					EmailPreferences = new EmailPreferences
					{
						DailyUpdate = true,
						WeeklyUpdate = true,
						NewNotifications = true,
					}
				}
			};
			var response = await Request($"{UserService.RootURL}/{user.Guid}", EHTTPRequestType.PATCH, patchObj);
			Assert.IsTrue(response.IsSuccessStatusCode);
			user = ResourceUtility.GetManager<User>().GetSingle(x => x.Guid == user.Guid);
			Assert.AreEqual(newName, user.Name);
			Assert.IsTrue(user.PersonalData.EmailPreferences.WeeklyUpdate);
			Assert.IsTrue(user.PersonalData.EmailPreferences.DailyUpdate);
			Assert.IsTrue(user.PersonalData.EmailPreferences.NewNotifications);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Update a User" },
				LastRequest);
		}

		[TestMethod]
		public async Task CanAccessOwnAccountPage()
		{
			var user = GetRandomUser(out var password, out _);
			await Login(user.Slug, password);
			var response = await RequestJSON($"{UserService.RootURL}", EHTTPRequestType.GET);
			Assert.AreEqual(user.Name, response.Value<string>("name"));
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Get The Logged In User" },
				LastRequest);
		}

		[TestMethod]
		public async Task AccessAuthEndpointWithoutLoginReturnsForbid()
		{
			await AssertX.ThrowsAsync<Exception>(RequestJSON($"{UserService.RootURL}", EHTTPRequestType.GET),
				e => e.Message.Contains("StatusCode: 302, ReasonPhrase: 'Found'"));

		}

		[TestMethod]
		public async Task CanLogin()
		{
			var user = GetRandomUser(out var password, out _);
			await Login(user.Slug, password);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Login" },
				LastRequest);
			var response = await RequestJSON($"{UserService.RootURL}/{user.Guid}", EHTTPRequestType.GET);
			Assert.AreEqual(user.Name, response.Value<string>("name"));
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Login" },
				LastRequest);
		}

		[TestMethod]
		public async Task CanGetNotifications()
		{
			var user = GetRandomUser(out var password, out _);
			await Login(user.Slug, password);
			var request = await RequestJSON<JObject>($"{UserService.RootURL}/notifications", EHTTPRequestType.GET);
			var notifications = request.Value<JArray>("notifications").Values<JToken>().Select(r => r.ToObject<Notification>());
			Assert.IsTrue(notifications.Any());

			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = $"Get Notifications" }, LastRequest);
		}

		[TestMethod]
		public async Task CanLogout()
		{
			var user = GetRandomUser(out var password, out _);
			await Login(user.Slug, password);
			await Request($"{UserService.RootURL}/{user.Guid}", EHTTPRequestType.GET);
			await Logout();
			await AssertX.ThrowsAsync<Exception>(Request($"{UserService.RootURL}/{user.Guid}", EHTTPRequestType.GET),
				e => e.Message.Contains("StatusCode: 302, ReasonPhrase: 'Found'"));
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Logout" },
				LastRequest);
		}

		[TestMethod]
		public async Task CanDeleteProfile()
		{
			var user = GetRandomUser(out var password, out _);
			await Login(user.Slug, password);
			await Request($"{UserService.RootURL}/{user.Guid}", EHTTPRequestType.DELETE);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Delete a User" },
				LastRequest);
		}

		[TestMethod]
		public async Task CannotLoginWithBadAuth()
		{
			var user = GetRandomUser(out var password, out _);
			await AssertX.ThrowsAsync<Exception>(Login(user.Slug, "not the password"),
				e => e.Message.Contains("Unauthorized"));
		}

		[TestMethod]
		public async virtual Task CannotGetAnonymously()
		{
			var user = GetRandomUser(out _, out _);
			await AssertX.ThrowsAsync<Exception>(RequestJSON($"{UserService.RootURL}/{user.Guid.ToString()}", EHTTPRequestType.GET));
		}

		[TestMethod]
		public async virtual Task CanGetOwnProfile()
		{
			var user = GetRandomUser(out var password, out var context);
			await Login(user.Slug, password);
			await RequestJSON($"{UserService.RootURL}/{user.Guid.ToString()}", EHTTPRequestType.GET);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Get a User" },
				LastRequest);
		}

		[TestMethod]
		public async Task CanResetPassword()
		{
			var user = GetRandomUser(out var password, out var context);
			var request = await Request($"{UserService.RootURL}/{UserService.RESET_PASSWORD}", EHTTPRequestType.GET,
				null, new[] { ("email", user.PersonalData.Email) });
			var newPassword = ValidationExtensions.GenerateStrongPassword();
			user = UserManager.GetSingle(u => u.Guid == user.Guid);
			var token = user.TokenCollection.GetSingleToken<ResetPasswordToken>(context, EPermissionLevel.OWNER, user);

			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Request a Password Reset Token" },
				request);

			request = await Request($"{UserService.RootURL}/{UserService.REDEEM_PASSWORD_TOKEN}", EHTTPRequestType.POST,
				newPassword, new[] { (UserService.PARAM_TOKEN, token.Key) });

			await Login(user.Slug, newPassword);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Redeem a Password Reset Token" },
				request);
		}

		[TestMethod]
		public async Task CanChangePassword()
		{
			var user = GetRandomUser(out var password, out var context);
			var newPassword = ValidationExtensions.GenerateStrongPassword();
			await Login(user.Slug, password);
			var req = await Request($"{UserService.RootURL}/{UserService.CHANGE_PASSWORD}", EHTTPRequestType.POST, new { currentpassword = password, newpassword = newPassword });
			await Login(user.Slug, newPassword);
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Change your password" },
				req);
		}

		[TestMethod]
		public async Task CanVerifyEmail()
		{
			var user = GetRandomUser(out var password, out var context, false);
			var token = user.TokenCollection.GetSingleToken<VerifyEmailToken>(context, EPermissionLevel.OWNER, user);
			Assert.IsNotNull(token);
			Assert.IsNotNull(token.Token);
			await Login(user.Slug, password);
			var request = await Request($"{UserService.RootURL}/{UserService.VERIFY_EMAIL}", EHTTPRequestType.GET,
				null, new[] { ("token", token.Token) });
			Postman.Update(
				new PostmanEntryAddress { Category = UserCat, Request = "Redeem an Email Verification Token" },
				LastRequest);
		}
	}
}
