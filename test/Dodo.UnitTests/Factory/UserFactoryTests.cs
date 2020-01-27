using Dodo;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REST;
using System;

namespace Factory
{
	[TestClass]
	public class UserFactoryTests : FactoryTestBase<User, UserSchema>
	{
		protected override AccessContext GetCreationContext()
		{
			return default;
		}

		protected override void VerifyCreatedObject(User obj, UserSchema schema)
		{
		}

		[Ignore]
		public override void CannotCreateWithBadAuth()
		{
		}
	}
}
