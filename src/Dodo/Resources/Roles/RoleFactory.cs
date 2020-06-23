using Common.Extensions;
using Dodo.DodoResources;
using Resources;
using Resources.Serializers;
using System;

namespace Dodo.Roles
{
	public class RoleSerializer : ResourceReferenceSerializer<Role> { }

	public class RoleSchema : OwnedResourceSchemaBase
	{
		public RoleSchema() { }

		public RoleSchema(string name, string publicDescription, Guid parent) : base(name, publicDescription, parent)
		{
		}
	}

	public class RoleFactory : DodoResourceFactory<Role, RoleSchema>
	{
	}
}
