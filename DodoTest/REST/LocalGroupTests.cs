using Common;
using Dodo.LocalGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using RestSharp;

namespace RESTTests
{
	[TestClass]
	public class LocalGroupTests : RESTTestBase<LocalGroup>
	{
		public override string CreationURL => "newlocalgroup";

		[TestInitialize]
		public void Setup()
		{
			RequestJSON("register", Method.POST, new { Username = CurrentLogin, Password = CurrentPassword, Email = "" });
		}

		public override object GetCreationSchema()
		{
			return new { Name = "", Location = new GeoLocation(87, 14) };
		}

		public override object GetPatchSchema()
		{
			return new { Location = new GeoLocation(14, 87) };
		}
	}
}
