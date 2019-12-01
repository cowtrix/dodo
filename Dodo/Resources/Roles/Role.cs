using Common;
using Common.Extensions;
using Common.Security;
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
		[View(EPermissionLevel.ADMIN)]
		public ResourceReference<GroupResource> Parent { get; set; }
		[View(EPermissionLevel.USER)]
		public string Mandate { get; set; }

		public override string ResourceURL => $"{Parent.Value.ResourceURL}/{ROOT}/{Name.StripForURL()}";

		public Role() : base() { }

		public Role(GroupResource parent, RoleRESTHandler.CreationSchema schema) : base(parent.Creator, schema.Name)
		{
			Parent = new ResourceReference<GroupResource>(parent);
			Mandate = schema.Mandate;
		}

		public override bool IsAuthorised(User requestOwner, Passphrase passphrase, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			if (request.Method != SimpleHttpServer.EHTTPRequestType.GET)
			{
				if (Creator.Guid == requestOwner.GUID)
				{
					permissionLevel = EPermissionLevel.OWNER;
					return true;
				}
				if (Parent.Value.IsAdmin(requestOwner, passphrase))
				{
					permissionLevel = EPermissionLevel.ADMIN;
					return true;
				}
				permissionLevel = EPermissionLevel.PUBLIC;
				return false;
			}
			if (requestOwner != null)
			{
				permissionLevel = EPermissionLevel.USER;
				return true;
			}
			permissionLevel = EPermissionLevel.PUBLIC;
			return true;
		}
	}
}
