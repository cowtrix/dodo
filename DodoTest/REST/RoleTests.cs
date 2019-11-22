using Common;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;
using System;

namespace RESTTests
{
	[TestClass]
	public class RoleTests : RESTTestBase<Role>
	{
		public override string CreationURL => $"{WorkingGroupURL}/{Role.ROOT}/create";
		private string WorkingGroupURL { get; set; }

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			var rebellion = RequestJSON("rebellions/create", Method.POST,
				new RebellionRESTHandler.CreationSchema { Name = "Test Rebellion", Location = new GeoLocation(45, 97) });
			WorkingGroupURL = RequestJSON(rebellion.Value<string>("ResourceURL") + "/wg/create", Method.POST,
				new WorkingGroupRESTHandler.CreationSchema("Test Working Group"))
				.Value<string>("ResourceURL");
		}

		public override object GetCreationSchema()
		{
			return new RoleRESTHandler.CreationSchema { Name = "Test Role", Mandate = "Test mandate" };
		}

		public override object GetPatchSchema()
		{
			return new { Mandate = "New mandate" };
		}

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new RoleRESTHandler.CreationSchema { Name = "Create", Mandate = "Test mandate" }),
				e => e.Message.Contains("Reserved Resource URL"));
		}
	}
}
