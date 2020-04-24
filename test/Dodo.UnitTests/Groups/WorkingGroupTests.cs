using Dodo.Roles;
using Dodo.SharedTest;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;

namespace Groups
{
	[TestClass]
	public class WorkingGroupTests : GroupResourceTestBase<WorkingGroup> 
	{
		[TestMethod]
		public void CanAddAndRemoveSubWorkingGroup()
		{
			GenerateUser(SchemaGenerator.GetRandomUser(default), out var creatorContext);
			var baseWg = ResourceUtility.GetFactory<WorkingGroup>()
				.CreateTypedObject(creatorContext, SchemaGenerator.GetRandomSchema<WorkingGroup>(creatorContext));

			var subWgSchema = SchemaGenerator.GetRandomSchema<WorkingGroup>(creatorContext) as WorkingGroupSchema;
			subWgSchema.Parent = baseWg.Guid;
			var subWg = ResourceUtility.GetFactory<WorkingGroup>().CreateTypedObject(creatorContext, subWgSchema);
			Assert.IsTrue((subWg.Parent.GetValue() as WorkingGroup).WorkingGroups.Contains(subWg.Guid),
				"Working Group was not included in Rebellion list after creation");
			ResourceUtility.GetManager<WorkingGroup>().Delete(subWg);
			Assert.IsFalse((subWg.Parent.GetValue() as WorkingGroup).WorkingGroups.Contains(subWg.Guid),
				"Working Group was not renoved from Rebellion list after deletion");
		}

		[TestMethod]
		public void CanAddAndRemoveRoles()
		{
			GenerateUser(SchemaGenerator.GetRandomUser(default), out var creatorContext);
			var role = ResourceUtility.GetFactory<Role>().CreateTypedObject(creatorContext, 
				SchemaGenerator.GetRandomSchema<Role>(creatorContext));
			Assert.IsTrue((role.Parent.GetValue() as WorkingGroup).Roles.Contains(role.Guid),
				"Working Group was not included in Rebellion list after creation");
			ResourceUtility.GetManager<Role>().Delete(role);
			Assert.IsFalse((role.Parent.GetValue() as WorkingGroup).Roles.Contains(role.Guid),
				"Working Group was not renoved from Rebellion list after deletion");
		}
	}
}
