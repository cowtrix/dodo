﻿using Dodo;
using Dodo.Roles;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources.Security;

namespace Factory
{
	[TestClass]
	public class RoleFactoryTests : FactoryTestBase<Role, RoleSchema>
	{
		protected override void VerifyCreatedObject(Role obj, RoleSchema schema)
		{
		}
	}
}
