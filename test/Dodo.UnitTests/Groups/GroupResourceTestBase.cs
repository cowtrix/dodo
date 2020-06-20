using Dodo;
using Dodo.SharedTest;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using Resources.Security;
using SharedTest;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Groups
{
	public abstract class GroupResourceTestBase<T> : TestBase where T : GroupResource
	{
		protected IResourceManager<T> ResourceManager => ResourceUtility.GetManager<T>();

		[TestMethod]

		public void CanJoinAndLeave()
		{
			var creator = GenerateUser(SchemaGenerator.GetRandomUser(default), out var creatorContext);
			var newGroup = CreateObject<T>(creatorContext, SchemaGenerator.GetRandomSchema<T>(creatorContext));

			// Join
			var user = GenerateUser(SchemaGenerator.GetRandomUser(default), out var joinerContext);
			using (var rscLock = new ResourceLock(newGroup))
			{
				newGroup = rscLock.Value as T;
				newGroup.Members.Add(user, joinerContext.Passphrase);
				Assert.IsTrue(newGroup.Members.IsAuthorised(user, joinerContext.Passphrase));
				ResourceManager.Update(newGroup, rscLock);
			}
			var updatedGroup = ResourceManager.GetSingle(g => g.Guid == newGroup.Guid);
			Assert.IsTrue(updatedGroup.Members.IsAuthorised(user, joinerContext.Passphrase));

			// Leave
			using (var rscLock = new ResourceLock(newGroup))
			{
				newGroup = rscLock.Value as T;
				newGroup.Members.Remove(user, joinerContext.Passphrase);
				Assert.IsFalse(newGroup.Members.IsAuthorised(user, joinerContext.Passphrase));
				ResourceManager.Update(newGroup, rscLock);
			}
			updatedGroup = ResourceManager.GetSingle(g => g.Guid == newGroup.Guid);
			Assert.IsFalse(updatedGroup.Members.IsAuthorised(user, joinerContext.Passphrase));
		}

		[TestMethod]
		[TestCategory("Administration")]
		public void CanAddAdmin()
		{
			var creator = GenerateUser(SchemaGenerator.GetRandomUser(default), out var creatorContext);
			var newGroup = CreateObject<T>(creatorContext, SchemaGenerator.GetRandomSchema<T>(creatorContext));

			// Add
			var newAdmin = GenerateUser(SchemaGenerator.GetRandomUser(default), out var newAdminContext);
			newGroup.AddAdmin(creatorContext, newAdmin);
			var updatedGroup = ResourceManager.GetSingle(g => g.Guid == newGroup.Guid);
			var all = ResourceManager.Get(r => true).ToList();
			Assert.IsTrue(updatedGroup.IsAdmin(newAdmin, creatorContext));
		}

		[TestMethod]
		public void CanGetNotifications()
		{
			GetRandomUser(out var pass, out var context);
			var rsc = CreateObject<T>(context);
			var notifications = rsc.GetNotifications(context, EPermissionLevel.OWNER);
			Assert.IsTrue(notifications.Any());
		}
	}
}
