using Common;
using Dodo.Roles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace RESTTests
{
	[TestClass]
	public class RoleTests : RESTTestBase<Role>
	{
		public override string CreationURL => "newrole";

		private string WorkingGroupGUID { get; set; }

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			var rebellion = RequestJSON("newrebellion", Method.POST, new { RebellionName = "Test Rebellion", Location = new GeoLocation(45, 97) })
				.Value<string>("GUID");
			WorkingGroupGUID = RequestJSON("newworkinggroup", Method.POST, new { RebellionGUID = rebellion, ParentWorkingGroupGUID = "", WorkingGroupName = "Test Working Group" })
				.Value<string>("GUID");
		}

		public override object GetCreationSchema()
		{
			return new RoleRESTHandler.CreationSchema { GroupGUID = WorkingGroupGUID, Name = "Test Role", Mandate = "Test mandate" };
		}

		public override object GetPatchSchema()
		{
			return new { Mandate = "New mandate" };
		}
	}
}
