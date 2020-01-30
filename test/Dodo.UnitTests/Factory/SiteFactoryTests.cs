﻿using Dodo;
using Dodo.Sites;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using REST.Security;

namespace Factory
{
	[TestClass]
	public class SiteFactoryTests : FactoryTestBase<Site, SiteSchema>
	{
		protected override void VerifyCreatedObject(Site obj, SiteSchema schema)
		{
		}
	}
}
