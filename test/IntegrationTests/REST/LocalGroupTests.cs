using Common;
using Common.Extensions;
using Dodo.LocalGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using SharedTest;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RESTTests
{
	[TestClass]
	public class LocalGroupTests : GroupResourceTestBase<LocalGroup>
	{
		public override string CreationURL => LocalGroup.RootURL;

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(out var defaultGuid, DefaultUsername, DefaultName, DefaultPassword, DefaultEmail);
			DefaultGUID = defaultGuid;
		}

		public override object GetCreationSchema(bool unique)
		{
			if (!unique)
			{
				return new LocalGroupSchema("Test Local Group ", "Test description", new GeoLocation(87, 14));
			}
			return new LocalGroupSchema ("Test Local Group " + StringExtensions.RandomString(6),
				"Test description", new GeoLocation(87, 14));
		}

		public override object GetPatchSchema()
		{
			return new { Location = new GeoLocation(14, 87) };
		}

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new LocalGroupSchema("Create", "Test description", new GeoLocation(87, 14))),
				e => e.Message.Contains("Reserved Resource URL"));
		}

		[TestMethod]
		public void CanList()
		{
			var objects = new List<JObject>()
			{
				RequestJSON(CreationURL, Method.POST, new LocalGroupSchema("Test1", "Test description", new GeoLocation(87, 14))),
				RequestJSON(CreationURL, Method.POST, new LocalGroupSchema("Test2", "Test description", new GeoLocation(87, 14))),
				RequestJSON(CreationURL, Method.POST, new LocalGroupSchema("Test3", "Test description", new GeoLocation(87, 14))),
				RequestJSON(CreationURL, Method.POST, new LocalGroupSchema("Test4", "Test description", new GeoLocation(87, 14))),
			};
			var guids = objects.Select(x => x.Value<string>("GUID"));
			var list = Request("localgroups", Method.GET);
			Assert.IsTrue(guids.All(guid => list.Content.Contains(guid)));
			m_postman.UpdateExampleJSON(list.Content, "Local Groups", "List all Local Groups");
		}

		protected override void CheckCreatedObject(JObject obj)
		{
			base.CheckCreatedObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Local Groups", "Create a new Local Group");
		}

		protected override void CheckGetObject(JObject obj)
		{
			base.CheckGetObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Local Groups", "Get a Local Group");
		}

		protected override void CheckPatchedObject(JObject obj)
		{
			base.CheckPatchedObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Local Groups", "Update a Local Group");
		}
	}
}
