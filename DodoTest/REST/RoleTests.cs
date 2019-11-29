using Common;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;

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
			RegisterUser(DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			var rebellion = RequestJSON("rebellions/create", Method.POST,
				new RebellionRESTHandler.CreationSchema { Name = "Test Rebellion", Location = new GeoLocation(45, 97) });
			WorkingGroup = RequestJSON(rebellion.Value<string>("ResourceURL") + "/wg/create", Method.POST,
				new WorkingGroupRESTHandler.CreationSchema("Test Working Group"));
		}

		public override object GetCreationSchema()
		{
			return new RoleRESTHandler.CreationSchema { Name = "Test Role", Mandate = "Test mandate" };
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
			Assert.AreEqual(WorkingGroup.Value<string>("GUID"), obj.Value<JObject>("Parent").Value<string>("Guid"));
		}

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new RoleRESTHandler.CreationSchema { Name = "Create", Mandate = "Test mandate" }),
				e => e.Message.Contains("Reserved Resource URL"));
		}
	}
}
