using Common;
using Dodo.LocalGroups;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			return new { Email = "newemail@web.com" };
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
		public void CannotCreateUserWithInvalidUsername()
		{
			AssertX.Throws<Exception>(() => RegisterUser(""),
				e => e.Message.Contains("Username must be at least 3 characters"));

			AssertX.Throws<Exception>(() => RegisterUser("aa"),
				e => e.Message.Contains("Username must be at least 3 characters"));

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
	}
}
