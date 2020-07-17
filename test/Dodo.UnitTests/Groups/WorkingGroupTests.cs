using Dodo.Roles;
using Dodo.SharedTest;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using System.Linq;

namespace Groups
{
	[TestClass]
	public class WorkingGroupTests : AdministratedGroupResourceTestBase<WorkingGroup> 
	{
		[TestMethod]
		[TestCategory("Child Objects")]
		[TestCategory("Administration")]
		public void CanAddAndRemoveSubWorkingGroup()
		{
			GenerateUser(SchemaGenerator.GetRandomUser(default), out var creatorContext);
			var baseWg = CreateObject<WorkingGroup>(creatorContext, SchemaGenerator.GetRandomSchema<WorkingGroup>(creatorContext));

			var subWgSchema = SchemaGenerator.GetRandomSchema<WorkingGroup>(creatorContext) as WorkingGroupSchema;
			subWgSchema.Parent = baseWg.Guid;
			var subWg = CreateObject<WorkingGroup>(creatorContext, subWgSchema);
			Assert.IsTrue((subWg.Parent.GetValue() as WorkingGroup).WorkingGroups.Any(g => g.Guid == subWg.Guid),
				"Working Group was not included in Rebellion list after creation");
			ResourceUtility.GetManager<WorkingGroup>().Delete(subWg);
			Assert.IsFalse((subWg.Parent.GetValue() as WorkingGroup).WorkingGroups.Any(g => g.Guid == subWg.Guid),
				"Working Group was not renoved from Rebellion list after deletion");
		}

		[TestMethod]
		[TestCategory("Child Objects")]
		[TestCategory("Administration")]
		public void CanAddAndRemoveRoles()
		{
			GenerateUser(SchemaGenerator.GetRandomUser(default), out var creatorContext);
			var role = CreateObject<Role>(creatorContext, 
				SchemaGenerator.GetRandomSchema<Role>(creatorContext));
			Assert.IsTrue((role.Parent.GetValue() as WorkingGroup).Roles.Any(g => g.Guid == role.Guid),
				"Working Group was not included in Rebellion list after creation");
			ResourceUtility.GetManager<Role>().Delete(role);
			Assert.IsFalse((role.Parent.GetValue() as WorkingGroup).Roles.Any(g => g.Guid == role.Guid),
				"Working Group was not renoved from Rebellion list after deletion");
		}
	}
}
