using Common;
using Common.Extensions;
using Dodo.Gateways;
using Dodo.Rebellions;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;

namespace RESTTests
{
	[TestClass]
	public class RebellionTests : GroupResourceTestBase<Rebellion>
	{
		public override string CreationURL => Rebellion.ROOT + "/create";
		public override object GetCreationSchema(bool unique = false)
		{
			if(!unique)
			{
				return new RebellionRESTHandler.CreationSchema("Test Rebellion", "Test description", new GeoLocation(45, 97));
			}
			return new RebellionRESTHandler.CreationSchema("Test Rebellion " + StringExtensions.RandomString(6), "Test description", new GeoLocation(45, 97));
		}

		public override object GetPatchSchema()
		{
			return new { Location = new GeoLocation(62, 41), StartDate = new DateTime(2019, 10, 7), 
				EndDate = new DateTime(2019, 10, 14) };
		}

		protected override void CheckPatchedObject(JObject obj)
		{
			Assert.AreEqual(new GeoLocation(62, 41), obj.Value<JObject>("Location").ToObject<GeoLocation>());
			Assert.AreEqual(new DateTime(2019, 10, 7), obj.Value<DateTime>("StartDate"));
			Assert.AreEqual(new DateTime(2019, 10, 14), obj.Value<DateTime>("StartDate"));
			base.CheckPatchedObject(obj);
		}

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(out var defaultGuid, DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			DefaultGUID = defaultGuid;
		}

		[TestMethod]
		public void CannotCreateNewRebellionIfNotLoggedIn()
		{
			AssertX.Throws<Exception>(() =>
			{
				RequestJSON(CreationURL, Method.POST, GetCreationSchema(), "", "");
			}, e => e.Message.Contains("Forbidden"));
		}

		/*[TestMethod]
		public void CannotPatchWithoutAdminRights()
		{
			var rebellion = CreateNewRebellion("Test rebellion", new GeoLocation());
			var newUser = RegisterRandomUser(out var username, out _, out var password, out _, out _);
			AssertX.Throws<Exception>(() => RequestJSON(rebellion.Value<string>("ResourceURL"), Method.PATCH, new
			{
				BotConfiguration = new RebellionBotConfiguration()
				{
					TelegramConfig = new RebellionBotConfiguration.TelegramConfiguration()
				}
			}, username, password), (e) => e.Message.Contains("Forbidden"));
		}*/

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new RebellionRESTHandler.CreationSchema("Create", "Test description", new GeoLocation())),
				e => e.Message.Contains("Reserved Resource URL"));
		}
	}
}
