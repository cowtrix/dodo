using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RESTTests
{
	[TestClass]
	public class UserTests : RESTTestBase<User>
	{
		public override string CreationURL => "register";

		public override object GetCreationSchema()
		{
			return new { Username = CurrentLogin, Password = CurrentPassword, Email = "" };
		}

		public override object GetPatchSchema()
		{
			return new { Email = "test@web.com" };
		}
	}
}
