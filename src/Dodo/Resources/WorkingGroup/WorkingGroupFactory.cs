using Dodo.DodoResources;
using Resources;
using Resources.Serializers;
using System;

namespace Dodo.WorkingGroups
{
	public class WorkingGroupSerializer : ResourceReferenceSerializer<WorkingGroup> { }

	public class WorkingGroupSchema : OwnedResourceSchemaBase
	{
		public WorkingGroupSchema()
		{
		}

		public WorkingGroupSchema(string name, string description, string parent) 
			: base(name, description, parent)
		{
		}

		public override Type GetResourceType() => typeof(WorkingGroup);
	}

	public class WorkingGroupFactory : DodoResourceFactory<WorkingGroup, WorkingGroupSchema>
	{
	}
}
