using Common.Extensions;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace DodoTest.Serialization
{
	[TestClass]
	public class UserSerializationTests : SerializationTestBase<User>
	{
		protected override User GetObject()
		{
			return new User(new UserRESTHandler.CreationSchema("testuserrrr", ValidationExtensions.GenerateStrongPassword(), "Test Name", "test@test.com"));
		}
	}
}
