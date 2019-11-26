using Common;
using Dodo.Rebellions;
using Dodo.Users;
using Dodo.WorkingGroups;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;

namespace Dodo.Roles
{
	public class Role : DodoResource
	{
		public const string ROOT = "roles";
		[NoPatch]
		[View(EUserPriviligeLevel.ADMIN)]
		public ResourceReference<GroupResource> ParentGroup { get; set; }
		[View(EUserPriviligeLevel.USER)]
		public string Name { get; set; }
		[View(EUserPriviligeLevel.USER)]
		public string Mandate { get; set; }

		public override string ResourceURL => $"{ParentGroup.Value.ResourceURL}/{ROOT}/{Name.StripForURL()}";

		public Role(GroupResource parent, string roleName, string mandate) : base(parent.Creator)
		{
			ParentGroup = new ResourceReference<GroupResource>(parent);
			Name = roleName;
			Mandate = mandate;
		}

		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EUserPriviligeLevel permissionLevel)
		{
			return ParentGroup.Value.IsAuthorised(requestOwner, request, out permissionLevel);
		}
	}
}
