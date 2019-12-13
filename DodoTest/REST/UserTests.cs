using Common;
using Common.Extensions;
using Dodo;
using Dodo.LocalGroups;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SimpleHttpServer.REST;
using System;

namespace RESTTests
{
	[TestClass]
	public class UserTests : RESTTestBase<User>
	{
		public override string CreationURL => "register";

		public override object GetCreationSchema(bool unique)
		{
			if(unique)
			{
				return new UserRESTHandler.CreationSchema(
					DefaultUsername + StringExtensions.RandomString(6).ToLowerInvariant(),
					DefaultPassword,
					DefaultName,
					DefaultEmail
				);
			}
			return new UserRESTHandler.CreationSchema(
					DefaultUsername,
					DefaultPassword,
					DefaultName,
					DefaultEmail
				);
		}

		public override object GetPatchSchema()
		{
			return new { Name = "John Doe" };
		}

		[TestMethod]
		public override void CannotPatchDuplicate()
		{
			var firstObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema(false));
			var secondObj = RegisterRandomUser(out var username, out var originalName, out var password, out _, out _);
			AssertX.Throws<Exception>(() =>
				RequestJSON(secondObj.Value<string>("ResourceURL"), Method.PATCH,
				new { WebAuth = new { Username = firstObj.Value<JObject>("WebAuth").Value<string>("Username") } }, username, password),
				e => e.Message.Contains("Duplicate ResourceURL"));
			RequestJSON(secondObj.Value<string>("ResourceURL"), Method.GET, null, username, password);
		}

		[TestMethod]
		public void CanResetPassword()
		{
			var user = RegisterUser(out var guid);
			var request = Request(UserRESTHandler.RESETPASS_URL, Method.POST, DefaultEmail, "", "");
			Assert.IsTrue(request.Content.Contains("If an account with that email exists, a password reset email has been sent"));
			var newPassword = ValidationExtensions.GenerateStrongPassword();
			var token = (ResourceUtility.GetResourceByGuid(Guid.Parse(guid)) as User)
				.PushActions.GetSinglePushAction<ResetPasswordAction>().TemporaryToken;
			request = Request(UserRESTHandler.RESETPASS_URL + "?token=" + token.Value, Method.POST, newPassword, "", "");
			Assert.IsTrue(request.Content.Contains("You've succesfully changed your password."));
			RequestJSON(user.Value<string>("ResourceURL"), Method.GET, null, DefaultUsername, newPassword);
		}

		[TestMethod]
		public void ResetPasswordBadActorScenario()
		{
			//User signs up
			var user1 = RegisterRandomUser(out var username1, out _, out var password1, out var email1, out var guid1);
			// User requests reset token
			var request = Request(UserRESTHandler.RESETPASS_URL, Method.POST, email1, "", "");
			Assert.IsTrue(request.Content.Contains("If an account with that email exists, a password reset email has been sent"));
			var newPassword = ValidationExtensions.GenerateStrongPassword();
			var token = (ResourceUtility.GetResourceByGuid(Guid.Parse(guid1)) as User)
				.PushActions.GetSinglePushAction<ResetPasswordAction>().TemporaryToken;
			// Register a second user
			var user2 = RegisterRandomUser(out var username2, out _, out var password2, out _, out var guid2);
			// Second user attempts to use the token - should be forbidden
			request = Request(UserRESTHandler.RESETPASS_URL + "?token=" +
				token.Value, Method.POST, newPassword, username2, password2);
			Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, request.StatusCode);
		}

		[TestMethod]
		public void ResetPasswordBadBehaviourScenario()
		{
			// Sending invalid emai should result in error
			var request = Request(UserRESTHandler.RESETPASS_URL, Method.POST, "not an email", "", "");
			Assert.AreEqual("Invalid email address", request.StatusDescription);

			// User signs up
			var user1 = RegisterRandomUser(out var username1, out _, out var password1, out var email1, out var guid1);
			// User requests reset token
			request = Request(UserRESTHandler.RESETPASS_URL, Method.POST, email1, "", "");
			Assert.IsTrue(request.Content.Contains("If an account with that email exists, a password reset email has been sent"));

			// User requests another token
			request = Request(UserRESTHandler.RESETPASS_URL, Method.POST, email1, "", "");
			Assert.IsTrue(request.Content.Contains("If an account with that email exists, a password reset email has been sent"));

			var token = (ResourceUtility.GetResourceByGuid(Guid.Parse(guid1)) as User)
				.PushActions.GetSinglePushAction<ResetPasswordAction>().TemporaryToken;
			// Try to change to a bad password
			request = Request(UserRESTHandler.RESETPASS_URL + "?token=" + token.Value, Method.POST, "badpass", "", "");
			Assert.IsTrue(request.Content.Contains("Password should be between 8 and 20 characters"));

			// Try to change to the same pass
			request = Request(UserRESTHandler.RESETPASS_URL + "?token=" + token.Value, Method.POST, password1, "", "");
			Assert.IsTrue(request.Content.Contains("Cannot use same password."));

			var newPassword = ValidationExtensions.GenerateStrongPassword();
			// Register a second user
			var user2 = RegisterRandomUser(out var username2, out _, out var password2, out _, out var guid2);
			// Second user attempts to use the token - should be forbidden
			request = Request(UserRESTHandler.RESETPASS_URL + "?token=" + token.Value, Method.POST, newPassword, username2, password2);
			Assert.AreEqual(System.Net.HttpStatusCode.Forbidden, request.StatusCode);
		}

		protected override void CheckCreatedObject(JObject obj)
		{
			Assert.IsTrue(obj.GetValue("WebAuth").Value<string>("Username") == DefaultUsername);
			Assert.IsTrue(obj.Value<string>("Email") == DefaultEmail);
			Assert.IsTrue(obj.Value<string>("Name") == DefaultName);

			VerifyUser(obj.Value<string>("GUID"), DefaultUsername, DefaultPassword, DefaultEmail);
		}

		[TestMethod]
		public override void CanDestroy()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema(false));
			VerifyUser(obj.Value<string>("GUID"), DefaultUsername, DefaultPassword, DefaultEmail);
			var response = Request(obj.Value<string>("ResourceURL"), Method.DELETE);
			Assert.IsTrue(response.StatusDescription.Contains("Resource deleted"));
		}

		[TestMethod]
		public override void CanPatch()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema(false));
			VerifyUser(obj.Value<string>("GUID"), DefaultUsername, DefaultPassword, DefaultEmail);
			var patch = RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, GetPatchSchema());
			Assert.AreNotEqual(obj.ToString(), patch.ToString());
			CheckPatchedObject(patch);
		}

		[TestMethod]
		public override void CannotPatchInvalid()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema(false));
			VerifyUser(obj.Value<string>("GUID"), DefaultUsername, DefaultPassword, DefaultEmail);
			AssertX.Throws<Exception>(() => RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, new { FakeField = "Not a field" }),
				x => x.Message.Contains("Invalid field names"));
		}

		protected override void CheckGetObject(JObject obj)
		{
			CheckCreatedObject(obj);
		}

		protected override void CheckPatchedObject(JObject obj)
		{
			Assert.IsTrue(obj.Value<string>("Name") == "John Doe");
		}

		[TestMethod]
		public void CannotCreateUserWithInvalidEmail()
		{
			AssertX.Throws<Exception>(() => RegisterUser(out _, email: ""),
				e => e.Message.Contains("Invalid email"));

			AssertX.Throws<Exception>(() => RegisterUser(out _, email: "not an email"),
				e => e.Message.Contains("Invalid email"));

			AssertX.Throws<Exception>(() => RegisterUser(out _, email: "myemail @gmail.com"),
				e => e.Message.Contains("Invalid email"));
		}

		[TestMethod]
		public void CannotCreateWithInvalidName()
		{
			AssertX.Throws<Exception>(() => RegisterUser(out _, name: ""),
				e => e.Message.Contains("Name length must be between "));

			AssertX.Throws<Exception>(() => RegisterUser(out _, name: StringExtensions.RandomString(256)),
				e => e.Message.Contains("Name length must be between "));

			AssertX.Throws<Exception>(() => RegisterUser(out _, name: "I'm a coordinator"),
				e => e.Message.Contains("Name contains reserved word: COORDINATOR"));

			AssertX.Throws<Exception>(() => RegisterUser(out _, name: "System Administrator"),
				e => e.Message.Contains("Name contains reserved word: ADMIN"));
		}

		[TestMethod]
		public void CannotCreateUserWithInvalidUsername()
		{
			AssertX.Throws<Exception>(() => RegisterUser(out _, ""),
				e => e.Message.Contains("Name length must be between "));

			AssertX.Throws<Exception>(() => RegisterUser(out _, "aa"),
				e => e.Message.Contains("Name length must be between "));

			AssertX.Throws<Exception>(() => RegisterUser(out _, "invalid username"),
				e => e.Message.Contains("Username can only contain alphanumeric characters and _"));

			AssertX.Throws<Exception>(() => RegisterUser(out _, "@@@@"),
				e => e.Message.Contains("String was not escaped."));

			AssertX.Throws<Exception>(() => RegisterUser(out _, "AAAAA"),
				e => e.Message.Contains("Username was invalid, Expected: "));
		}

		[TestMethod]
		public void CannotCreateUserWithWeakPassword()
		{
			AssertX.Throws<Exception>(() => RegisterUser(out _, password: ""),
				e => e.Message.Contains("Password should not be empty"));

			AssertX.Throws<Exception>(() => RegisterUser(out _, password: "a@c"),
				e => e.Message.Contains("Password should be between"));

			AssertX.Throws<Exception>(() => RegisterUser(out _, password: "#" + StringExtensions.RandomString(256)),
				e => e.Message.Contains("Password should be between"));

			AssertX.Throws<Exception>(() => RegisterUser(out _, password: "password"),
				e => e.Message.Contains("Password should contain at least one symbol"));
		}

		[TestMethod]
		public void CannotPatchUserWithInvalidName()
		{
			var obj = RegisterUser(out _);

			AssertX.Throws<Exception>(() => RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, new { Name = "" }),
				e => e.Message.Contains("Name length must be between "));

			AssertX.Throws<Exception>(() => RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, new { Name = StringExtensions.RandomString(256) }),
				e => e.Message.Contains("Name length must be between "));
		}
	}
}
