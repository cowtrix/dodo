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
			RequestJSON("register", Method.POST, new { Username = CurrentLogin, Password = CurrentPassword, Email = "" });
			Rebellion = RequestJSON("newrebellion", Method.POST, new { RebellionName = "Test Rebellion", Location = new GeoLocation(45, 97) })
				.Value<string>("GUID");
		}

		public override object GetCreationSchema()
		{
			return new { RebellionGUID = Rebellion, WorkingGroupName = "Test Rebellion" };
		}

		public override object GetPatchSchema()
		{
			return new { Mandate = "This is a test mandate" };
		}
	}
}
