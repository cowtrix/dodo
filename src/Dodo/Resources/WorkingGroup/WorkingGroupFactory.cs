using Dodo.DodoResources;
using Resources;
using Resources.Serializers;
using System;

namespace Dodo.WorkingGroups
{
	//public class WorkingGroupSerializer : ResourceReferenceSerializer<WorkingGroup> { }

	public class WorkingGroupSchema : OwnedResourceSchemaBase
	{
		public WorkingGroupSchema()
		{
		}

		public WorkingGroupSchema(string name, string description, Guid parent)
		{
			Name = name;
			PublicDescription = description;
			Parent = parent;
		}
	}

	public class WorkingGroupFactory : DodoResourceFactory<WorkingGroup, WorkingGroupSchema>
	{
	}
}
