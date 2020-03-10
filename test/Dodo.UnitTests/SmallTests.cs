using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using SharedTest;
using System;
using System.Collections.Generic;
using System.Text;
using Dodo;
using Common.Extensions;
using Dodo.Users;
using Dodo.Rebellions;
using Dodo.LocalGroups;
using Dodo.WorkingGroups;

namespace Dodo.UnitTests
{
	public abstract class ResourceReferenceTests<T> : TestBase where T : class, IRESTResource
	{
		class ResourceRefClass : IVerifiable
		{
			[NotNulResource]
			public ResourceReference<T> Ref;

			public bool CanVerify() => true;
		}

		[TestMethod]
		public void VerifyNotNullResource()
		{
			var rsc = CreateObject<T>();
			var reference = new ResourceRefClass()
			{
				Ref = new ResourceReference<T>(rsc)
			};
			Assert.IsTrue(reference.Verify(out var error), error);
		}

		[TestMethod]
		public void VerifyNullResourceThrows()
		{
			var reference = new ResourceRefClass(); ;
			Assert.IsFalse(reference.Verify(out _), "Reference thought it had a value");
		}
	}

	[TestClass]
	public class UserResourceReferenceTests : ResourceReferenceTests<User> { }
	[TestClass]
	public class RebellionResourceReferenceTests : ResourceReferenceTests<Rebellion> { }
	[TestClass]
	public class LocalGroupResourceReferenceTests : ResourceReferenceTests<LocalGroup> { }
	[TestClass]
	public class WorkingGroupResourceReferenceTests : ResourceReferenceTests<WorkingGroup> { }

}
