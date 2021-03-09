using Dodo;
using Dodo.SharedTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using System.Linq;

namespace Groups
{
	public abstract class AdministratedGroupResourceTestBase<T> : GroupResourceTestBase<T> where T : AdministratedGroupResource
	{
		[TestMethod]
		[TestCategory("Administration")]
		public void CanAddAdmin()
		{
			var creator = GenerateUser(SchemaGenerator.GetRandomUser(default), out var creatorContext);
			var newGroup = CreateObject<T>(creatorContext, SchemaGenerator.GetRandomSchema<T>(creatorContext));

			// Add
			var newAdmin = GenerateUser(SchemaGenerator.GetRandomUser(default), out var newAdminContext);
			using (var rscLock = new ResourceLock(newGroup))
			{
				Assert.IsTrue(newGroup.AddNewAdmin(creatorContext, newAdmin));
				ResourceManager.Update(newGroup, rscLock);
			}
			var updatedGroup = ResourceManager.GetSingle(g => g.Guid == newGroup.Guid);
			var all = ResourceManager.Get(r => true).ToList();
			Assert.IsTrue(updatedGroup.IsAdmin(newAdmin, creatorContext, out var p));
			Assert.IsFalse(p.CanEditAdministrators);
		}
	}
}
