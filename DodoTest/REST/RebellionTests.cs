using Common;
using Dodo.Gateways;
using Dodo.Rebellions;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System;

namespace RESTTests
{
	[TestClass]
	public class RebellionTests : RESTTestBase<Rebellion>
	{
		public override string CreationURL => "newrebellion";
		public override object GetCreationSchema()
		{
			return new { RebellionName = "Test Rebellion", Location = new GeoLocation(45, 97) };
		}

		[TestInitialize]
		public void Setup()
		{
			RequestJSON("register", Method.POST, new { Username = CurrentLogin, Password = CurrentPassword, Email = "" });
		}

		[TestMethod]
		public void CannotCreateNewRebellionIfNotLoggedIn()
		{
			AssertX.Throws<Exception>(() =>
			{
				RequestJSON(CreationURL, Method.POST, GetCreationSchema(), "", "");
			}, e => e.Message.Contains("You need to login"));
		}

		[TestMethod]
		public void CannotPatchProtectedField()
		{
			var rebellion = CreateNewRebellion("Test rebellion", new GeoLocation());
			var newUser = RegisterUser("Second user", "password");
			RequestJSON(rebellion.Value<string>("ResourceURL"), Method.PATCH, new
			{
				BotConfiguration = new RebellionBotConfiguration()
				{
					TelegramConfig = new RebellionBotConfiguration.TelegramConfiguration()
				}
			}, "Second user", "password");
		}

		public override object GetPatchSchema()
		{
			return new { Location = new GeoLocation(62, 41) };
		}
	}
}
