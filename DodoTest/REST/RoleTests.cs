using Common;
using Common.Extensions;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Linq;

namespace RESTTests
{
	[TestClass]
	public class RoleTests : RESTTestBase<Role>
	{
		public override string CreationURL => $"{WorkingGroup.Value<string>("ResourceURL")}/{Role.ROOT}/create";
		private JObject WorkingGroup { get; set; }

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(out _, DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			var rebellion = RequestJSON("rebellions/create", Method.POST,
				new RebellionRESTHandler.CreationSchema("Test Rebellion", new GeoLocation(45, 97)));
			WorkingGroup = RequestJSON(rebellion.Value<string>("ResourceURL") + "/wg/create", Method.POST,
				new WorkingGroupRESTHandler.CreationSchema("Test Working Group", "Test mandate"));
		}

		public override object GetCreationSchema(bool unique)
		{
			if(!unique)
			{
				return new RoleRESTHandler.CreationSchema
				{
					Name = "Test Role ",
					Mandate = "Test mandate"
				};
			}
			return new RoleRESTHandler.CreationSchema { Name = "Test Role " + StringExtensions.RandomString(6),
				Mandate = "Test mandate" };
		}

		public override object GetPatchSchema()
		{
			return new { Mandate = "New mandate" };
		}

		protected override void CheckPatchedObject(JObject obj)
		{
			Assert.AreEqual("New mandate", obj.Value<string>("Mandate"));
		}

		protected override void CheckCreatedObject(JObject obj)
		{
			var workingGroupGUID = WorkingGroup.Value<string>("GUID");
			var workingGroupGUIDFromObj = obj.Value<JObject>("Parent").Value<string>("Guid");
			Assert.AreEqual(workingGroupGUID, workingGroupGUIDFromObj);
			var parent = RequestJSON(WorkingGroup.Value<string>("ResourceURL"), Method.GET);
			Assert.IsTrue(parent.Value<JArray>("Roles").Values<string>().Contains(obj.Value<string>("GUID")));
		}

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new RoleRESTHandler.CreationSchema { Name = "Create", Mandate = "Test mandate" }),
				e => e.Message.Contains("Reserved Resource URL"));
		}
	}
}
