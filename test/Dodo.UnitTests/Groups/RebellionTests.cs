using Dodo.Rebellions;
using Dodo.SharedTest;
using Dodo.LocationResources;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using System.Linq;

namespace Groups
{
	[TestClass]
	public class RebellionTests : GroupResourceTestBase<Rebellion> 
	{
		[TestMethod]
		public void CanAddAndRemoveSubWorkingGroup()
		{
			GenerateUser(SchemaGenerator.GetRandomUser(default), out var creatorContext);
			var newWg = ResourceUtility.GetFactory<WorkingGroup>()
				.CreateTypedObject(creatorContext, SchemaGenerator.GetRandomSchema<WorkingGroup>(creatorContext));
			Assert.IsTrue((newWg.Parent.GetValue() as Rebellion).WorkingGroups.Any(s => s.Guid == newWg.Guid),
				"Working Group was not included in Rebellion list after creation");
			ResourceUtility.GetManager<WorkingGroup>().Delete(newWg);
			Assert.IsFalse((newWg.Parent.GetValue() as Rebellion).WorkingGroups.Any(s => s.Guid == newWg.Guid),
				"Working Group was not renoved from Rebellion list after deletion");
		}

		[TestMethod]
		public void CanAddAndRemoveSite()
		{
			GenerateUser(SchemaGenerator.GetRandomUser(default), out var creatorContext);
			var eventSite = ResourceUtility.GetFactory<Event>()
				.CreateTypedObject(creatorContext, SchemaGenerator.GetRandomSchema<Event>(creatorContext));
			Assert.IsTrue((eventSite.Parent.GetValue() as Rebellion).Sites.Any(s => s.Guid == eventSite.Guid),
				"Site was not included in Rebellion list after creation");
			ResourceUtility.GetManager<LocationResourceBase>().Delete(eventSite);
			Assert.IsFalse((eventSite.Parent.GetValue() as Rebellion).WorkingGroups.Any(s => s.Guid == eventSite.Guid),
				"Site was not renoved from Rebellion list after deletion");
		}
	}
}
