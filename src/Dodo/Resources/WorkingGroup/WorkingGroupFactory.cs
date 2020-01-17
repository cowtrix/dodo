using REST;
using REST.Serializers;

namespace Dodo.WorkingGroups
{
	public class WorkingGroupSerializer : ResourceReferenceSerializer<WorkingGroup> { }

	public class WorkingGroupSchema : GroupResourceSchemaBase
	{
		public WorkingGroupSchema(AccessContext context, string name, string publicDescription, GroupResource parent) 
			: base(context, name, publicDescription, parent)
		{
		}
	}

	public class WorkingGroupFactory : ResourceFactory<WorkingGroup, WorkingGroupSchema>
	{
	}
}