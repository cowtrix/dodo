using Common;
using Common.Extensions;
using Dodo.Gateways;
using Dodo.Rebellions;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

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
			return new { Location = new GeoLocation(62, 41), StartDate = new DateTime(2019, 10, 7, 0, 0, 0, DateTimeKind.Utc),
				EndDate = new DateTime(2019, 10, 14, 0, 0, 0, DateTimeKind.Utc) };
		}

		protected override void CheckPatchedObject(JObject obj)
		{
			Assert.AreEqual(new GeoLocation(62, 41), obj.Value<JObject>("Location").ToObject<GeoLocation>());
			Assert.AreEqual(new DateTime(2019, 10, 7), obj.Value<DateTime>("StartDate"));
			Assert.AreEqual(new DateTime(2019, 10, 14), obj.Value<DateTime>("EndDate"));
			base.CheckPatchedObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Rebellions", "Create a new Rebellion");
		}

		protected override void CheckGetObject(JObject obj)
		{
			base.CheckGetObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Rebellions", "Get a Rebellion");
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

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new RebellionRESTHandler.CreationSchema("Create", "Test description", new GeoLocation())),
				e => e.Message.Contains("Reserved Resource URL"));
		}

		[TestMethod]
		public void CanList()
		{
			var objects = new List<JObject>()
			{
				RequestJSON(CreationURL, Method.POST, new RebellionRESTHandler.CreationSchema("Test1", "Test description", new GeoLocation(27, 79.2))),
				RequestJSON(CreationURL, Method.POST, new RebellionRESTHandler.CreationSchema("Test2", "Test description", new GeoLocation(26.9, 79))),
				RequestJSON(CreationURL, Method.POST, new RebellionRESTHandler.CreationSchema("Test3", "Test description", new GeoLocation(27.2, 79.1))),
				RequestJSON(CreationURL, Method.POST, new RebellionRESTHandler.CreationSchema("Test4", "Test description", new GeoLocation(26.4, 78.7))),
			};
			var guids = objects.Select(x => x.Value<string>("GUID"));
			var list = Request("rebellions", Method.GET);
			Assert.IsTrue(guids.All(guid => list.Content.Contains(guid)));
			m_postman.UpdateExampleJSON(list.Content, "Rebellions", "List all rebellions");
		}
	}
}
