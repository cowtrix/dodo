using Common;
using Common.Extensions;
using Dodo.Rebellions;
using Dodo.Users;
using Dodo.Utility;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTTests
{
	[TestClass]
	public class WorkingGroupTests : GroupResourceTestBase<WorkingGroup>
	{
		public override string CreationURL => $"{Rebellion.Value<string>("ResourceURL")}/{WorkingGroup.ROOT}/create";

		private JObject Rebellion { get; set; }

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(out var defaultGuid, DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			DefaultGUID = defaultGuid;
			Rebellion = RequestJSON("rebellions/create", Method.POST,
				new RebellionRESTHandler.CreationSchema { Name = "Test Rebellion", Location = new GeoLocation(45, 97) });
		}

		public override object GetCreationSchema(bool unique)
		{
			if(unique)
			{
				return new WorkingGroupRESTHandler.CreationSchema("Test Working Group " + StringExtensions.RandomString(6),
				"Test mandate");
			}
			return new WorkingGroupRESTHandler.CreationSchema("Test Working Group ", 
				"Test mandate");
		}

		public override object GetPatchSchema()
		{
			return new { Mandate = "This is a test mandate" };
		}

		protected override void CheckPatchedObject(JObject obj)
		{
			Assert.AreEqual("This is a test mandate", obj.Value<string>("Mandate"));
		}

		protected override void CheckCreatedObject(JObject obj)
		{
			Assert.AreEqual(Rebellion.Value<string>("GUID"), obj.Value<JObject>("Parent").Value<string>("Guid"));
		}

		[TestMethod]
		public void CanCreateSubWorkingGroup()
		{
			var wg = RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test Working Group", "Test mandate"));
			var subwg = RequestJSON(wg.Value<string>("ResourceURL") + "/wg/create",
				Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test Working Group", "Test mandate"));
			Assert.AreEqual(wg.Value<string>("GUID"), subwg.Value<JObject>("Parent").Value<string>("Guid"));
			wg = RequestJSON(wg.Value<string>("ResourceURL"), Method.GET);
			var subGroups = wg.Value<JArray>("WorkingGroups").Values<string>();
			Assert.IsTrue(subGroups.Count() == 1);
			Assert.IsTrue(subGroups.All(x => x == subwg.Value<string>("GUID")));
		}

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Create", "Test mandate")),
				e => e.Message.Contains("Reserved Resource URL"));
		}

		[TestMethod]
		public void CanList()
		{
			var objects = new List<JObject>()
			{
				RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test1", "Test mandate")),
				RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test2", "Test mandate")),
				RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test3", "Test mandate")),
				RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test4", "Test mandate")),
			};
			var guids = objects.Select(x => x.Value<string>("GUID"));
			var list = Request($"{Rebellion.Value<string>("ResourceURL")}/{WorkingGroup.ROOT}/", Method.GET);
			Assert.IsTrue(guids.All(guid => list.Content.Contains(guid)));
		}
	}
}
