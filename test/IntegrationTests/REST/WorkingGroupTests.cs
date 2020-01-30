using Common;
using Common.Extensions;
using Dodo.Rebellions;
using Dodo.SharedTest;
using Dodo.Users;
using Dodo.Utility;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using SharedTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTTests
{
	[TestClass]
	public class WorkingGroupTests : GroupResourceTestBase<WorkingGroup>
	{
		public override string ResourceRoot => WorkingGroupController.RootURL;

		/*[TestInitialize]
		public void Setup()
		{
			RegisterUser(out var defaultGuid, DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			DefaultGUID = defaultGuid;
			Rebellion = RequestJSON("rebellions/create", Method.POST,
				new RebellionSchema("Test Rebellion", "Test description", new GeoLocation(45, 97), RebellionTests.DefaultStart, RebellionTests.DefaultEnd));
		}

		public override object GetCreationSchema(bool unique)
		{
			if(unique)
			{
				return new WorkingGroupSchema("Test Working Group " + StringExtensions.RandomString(6),	TestMandate, Rebellion.Value<Guid>("GUID"));
			}
			return new WorkingGroupSchema("Test Working Group",	TestMandate, Rebellion.Value<Guid>("GUID"));
		}

		public override object GetPatchSchema()
		{
			return new { Description = "This is a test description" };
		}

		protected override void CheckPatchedObject(JObject obj)
		{
			Assert.AreEqual("This is a test description", obj.Value<string>("Description"));
			m_postman.UpdateExampleJSON(obj.ToString(), "Working Groups", "Update a Working Group");
		}

		protected override void CheckCreatedObject(JObject obj)
		{
			Assert.AreEqual(Rebellion.Value<string>("GUID"), obj.Value<JObject>("Parent").Value<string>("Guid"));
			m_postman.UpdateExampleJSON(obj.ToString(), "Working Groups", "Create a new Working Group");
		}

		protected override void CheckGetObject(JObject obj)
		{
			base.CheckGetObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Working Groups", "Get a Working Group");
		}

		[TestMethod]
		public void CanCreateSubWorkingGroup()
		{
			var wg = RequestJSON(CreationURL, Method.POST, new WorkingGroupSchema("Test Working Group", TestMandate, Rebellion.Value<Guid>("GUID")));
			var subwg = RequestJSON(wg.Value<string>("ResourceURL") + "/wg/create",
				Method.POST, new WorkingGroupSchema("Test Working Group", TestMandate, wg.Value<Guid>("GUID")));
			Assert.AreEqual(wg.Value<string>("GUID"), subwg.Value<JObject>("Parent").Value<string>("Guid"));
			wg = RequestJSON(wg.Value<string>("ResourceURL"), Method.GET);
			var subGroups = wg.Value<JArray>("WorkingGroups").Values<string>();
			Assert.IsTrue(subGroups.Count() == 1);
			Assert.IsTrue(subGroups.All(x => x == subwg.Value<string>("GUID")));
			m_postman.UpdateExampleJSON(subwg.ToString(), "Working Groups", "Create a sub Working Group");
		}

		[TestMethod]
		public void CannotCreateForInvalidParent()
		{
			AssertX.Throws<Exception>(() => RequestJSON($"rebellions/{Guid.NewGuid()}",
				Method.POST, GetCreationSchema(false)),
				e => e.Message.Contains("NotFound"));

			AssertX.Throws<Exception>(() => RequestJSON($"workinggroups/{Guid.NewGuid()}",
				Method.POST, GetCreationSchema(false)),
				e => e.Message.Contains("NotFound"));
		}

		[TestMethod]
		public void CanList()
		{
			var objects = new List<JObject>()
			{
				RequestJSON(CreationURL, Method.POST, new WorkingGroupSchema("Test1", TestMandate, Rebellion.Value<Guid>("GUID"))),
				RequestJSON(CreationURL, Method.POST, new WorkingGroupSchema("Test2", TestMandate, Rebellion.Value<Guid>("GUID"))),
				RequestJSON(CreationURL, Method.POST, new WorkingGroupSchema("Test3", TestMandate, Rebellion.Value<Guid>("GUID"))),
				RequestJSON(CreationURL, Method.POST, new WorkingGroupSchema("Test4", TestMandate, Rebellion.Value<Guid>("GUID"))),
			};
			var guids = objects.Select(x => x.Value<string>("GUID"));
			var list = Request(WorkingGroupController.RootURL, Method.GET);
			Assert.IsTrue(guids.All(guid => list.Content.Contains(guid)));
			m_postman.UpdateExampleJSON(list.Content, "Working Groups", "List all Working Groups");
		}*/
	}
}