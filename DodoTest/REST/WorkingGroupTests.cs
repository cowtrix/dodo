using Common;
using Dodo.Users;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace RESTTests
{
	[TestClass]
	public class WorkingGroupTests : RESTTestBase<WorkingGroup>
	{
		public override string CreationURL => "newworkinggroup";

		private string Rebellion { get; set; }

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			Rebellion = RequestJSON("newrebellion", Method.POST, new { RebellionName = "Test Rebellion", Location = new GeoLocation(45, 97) })
				.Value<string>("GUID");
		}

		public override object GetCreationSchema()
		{
			return new WorkingGroupRESTHandler.CreationSchema { RebellionGUID = Rebellion, ParentGroup = "", WorkingGroupName = "Test Working Group" };
		}

		public override object GetPatchSchema()
		{
			return new { Mandate = "This is a test mandate" };
		}
	}
}
