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
			return new RebellionRESTHandler.CreationSchema { RebellionName = "Test Rebellion", Location = new GeoLocation(45, 97) };
		}

		public override object GetPatchSchema()
		{
			return new { Location = new GeoLocation(62, 41) };
		}

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(DefaultUsername, "Test User", DefaultPassword, "test@web.com");
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
			var newUser = RegisterRandomUser(out var username, out _, out var password, out _);
			AssertX.Throws<Exception>(() => RequestJSON(rebellion.Value<string>("ResourceURL"), Method.PATCH, new
			{
				BotConfiguration = new RebellionBotConfiguration()
				{
					TelegramConfig = new RebellionBotConfiguration.TelegramConfiguration()
				}
			}, username, password), (e) => e.Message.Contains("Insufficient privileges"));
		}
	}
}
