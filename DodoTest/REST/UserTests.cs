using Common;
using Common.Extensions;
using Dodo.LocalGroups;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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

		public override object GetCreationSchema()
		{
			return new UserRESTHandler.CreationSchema { Username = DefaultUsername, Name = DefaultName, Password = DefaultPassword, Email = DefaultEmail };
		}

		public override object GetPatchSchema()
		{
			return new { Name = "John Doe" };
		}

		protected override void CheckCreatedObject(JObject obj)
		{
			Assert.IsTrue(obj.GetValue("WebAuth").Value<string>("Username") == DefaultUsername);
			Assert.IsTrue(obj.Value<string>("Email") == DefaultEmail);
			Assert.IsTrue(obj.Value<string>("Name") == DefaultName);
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
			AssertX.Throws<Exception>(() => RegisterUser(email: ""),
				e => e.Message.Contains("Invalid email address"));

			AssertX.Throws<Exception>(() => RegisterUser(email: "not an email"),
				e => e.Message.Contains("Invalid email address"));

			AssertX.Throws<Exception>(() => RegisterUser(email: "myemail @gmail.com"),
				e => e.Message.Contains("Invalid email address"));
		}

		[TestMethod]
		public void CannotCreateWithInvalidName()
		{
			AssertX.Throws<Exception>(() => RegisterUser(name: ""),
				e => e.Message.Contains("Name length must be between "));

			AssertX.Throws<Exception>(() => RegisterUser(name: StringExtensions.RandomString(256)),
				e => e.Message.Contains("Name length must be between "));

			AssertX.Throws<Exception>(() => RegisterUser(name: "I'm a coordinator"),
				e => e.Message.Contains("Name contains reserved word: COORDINATOR"));

			AssertX.Throws<Exception>(() => RegisterUser(name: "System Administrator"),
				e => e.Message.Contains("Name contains reserved word: ADMIN"));
		}

		[TestMethod]
		public void CannotCreateUserWithInvalidUsername()
		{
			AssertX.Throws<Exception>(() => RegisterUser(""),
				e => e.Message.Contains("Name length must be between "));

			AssertX.Throws<Exception>(() => RegisterUser("aa"),
				e => e.Message.Contains("Name length must be between "));

			AssertX.Throws<Exception>(() => RegisterUser("invalid username"),
				e => e.Message.Contains("Username can only contain alphanumeric characters and _"));

			AssertX.Throws<Exception>(() => RegisterUser("@@@@"),
				e => e.Message.Contains("Username can only contain alphanumeric characters and _"));

			AssertX.Throws<Exception>(() => RegisterUser("AAAAA"),
				e => e.Message.Contains("Username was invalid, Expected: "));
		}

		[TestMethod]
		public void CannotCreateUserWithWeakPassword()
		{
			AssertX.Throws<Exception>(() => RegisterUser(password: ""),
				e => e.Message.Contains("Password should not be empty"));

			AssertX.Throws<Exception>(() => RegisterUser(password: "a@c"),
				e => e.Message.Contains("Password should be between"));

			AssertX.Throws<Exception>(() => RegisterUser(password: "#" + StringExtensions.RandomString(256)),
				e => e.Message.Contains("Password should be between"));

			AssertX.Throws<Exception>(() => RegisterUser(password: "password"),
				e => e.Message.Contains("Password should contain at least one symbol"));
		}

		[TestMethod]
		public void CannotPatchUserWithInvalidName()
		{
			var obj = RegisterUser();

			AssertX.Throws<Exception>(() => RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, new { Name = "" }),
				e => e.Message.Contains("Name length must be between "));

			AssertX.Throws<Exception>(() => RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, new { Name = StringExtensions.RandomString(256) }),
				e => e.Message.Contains("Name length must be between "));
		}
	}
}
