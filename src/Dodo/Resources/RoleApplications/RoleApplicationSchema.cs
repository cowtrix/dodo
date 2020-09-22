using Resources;
using System;
using Dodo.Roles;
using Dodo.Users.Tokens;

namespace Dodo.RoleApplications
{
	public class RoleApplicationSchema : ResourceSchemaBase
	{
		public RoleApplicationSchema()
		{
		}

		public RoleApplicationSchema(string name, Role role, ApplicationModel application) : base(name)
		{
			Role = role;
			ParentGroup = role.Parent.GetValue() as IAsymmCapableResource;
			Application = application;
		}

		public Role Role { get; set; }
		public ApplicationModel Application { get; set; }
		public IAsymmCapableResource ParentGroup { get; internal set; }

		public override Type GetResourceType() => typeof(RoleApplication);
	}
}
