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

		public RoleApplicationSchema(string name, Role role, string application) : base(name)
		{
			Role = role;
			ParentGroup = role.Parent.GetValue(true) as IAsymmCapableResource;
			Application = application;
		}

		public Role Role { get; set; }
		public string Application { get; set; }
		public IAsymmCapableResource ParentGroup { get; internal set; }

		public override Type GetResourceType() => typeof(RoleApplication);
	}
}
