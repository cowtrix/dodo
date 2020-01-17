using Common.Extensions;
using REST;
using REST.Serializers;

namespace Dodo.Roles
{
	public class RoleSerializer : ResourceReferenceSerializer<Role> { }

	public class RoleSchema : DodoResourceSchemaBase
	{
		public GroupResource Parent { get; private set; }
		public string PublicDescription { get; private set; }
		public RoleSchema(AccessContext context, string name, string publicDescription, GroupResource parent) : base(context, name)
		{
			PublicDescription = publicDescription;
			Parent = parent;
		}
	}

	public class RoleFactory : ResourceFactory<Role, RoleSchema>
	{
	}
}
