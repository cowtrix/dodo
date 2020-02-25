using Common.Extensions;
using Dodo.Resources;
using Resources;
using Resources.Serializers;
using System;

namespace Dodo.Roles
{
	public class RoleSerializer : ResourceReferenceSerializer<Role> { }

	public class RoleSchema : DodoResourceSchemaBase
	{
		public Guid Parent { get; set; }
		public string PublicDescription { get; set; }

		public RoleSchema() { }

		public RoleSchema(string name, string publicDescription, Guid parent) : base(name)
		{
			PublicDescription = publicDescription;
			Parent = parent;
		}
	}

	public class RoleFactory : DodoResourceFactory<Role, RoleSchema>
	{
	}
}
