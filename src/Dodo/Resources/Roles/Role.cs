using Common.Extensions;
using Resources.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using Resources.Serializers;
using System.Collections.Generic;

namespace Dodo.Roles
{

	public class Role : DodoResource
	{

		[NoPatch]
		[View(EPermissionLevel.PUBLIC)]
		public ResourceReference<GroupResource> Parent { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		public string PublicDescription { get; set; }
		[View(EPermissionLevel.USER)]
		public string MemberDescription { get; set; }
		[View(EPermissionLevel.ADMIN)]
		public string AdminDescription { get; set; }
		[View(EPermissionLevel.USER)]
		public UserCollection RoleHolders;

		public Role(AccessContext context, RoleSchema schema) : base(context, schema)
		{
			Parent = new ResourceReference<GroupResource>(schema.Parent);
			PublicDescription = schema.PublicDescription;
			RoleHolders = new UserCollection(new List<ResourceReference<User>>(), context);
		}

		public override bool IsAuthorised(AccessContext context, EHTTPRequestType requestType, out EPermissionLevel permissionLevel)
		{
			if (context.User == null)
			{
				permissionLevel = EPermissionLevel.PUBLIC;
				return true;
			}
			if (requestType != EHTTPRequestType.GET)
			{
				if (Creator.Guid == context.User.GUID)
				{
					permissionLevel = EPermissionLevel.OWNER;
					return true;
				}
				if (Parent.GetValue().IsAdmin(context.User, context))
				{
					permissionLevel = EPermissionLevel.ADMIN;
					return true;
				}
				permissionLevel = EPermissionLevel.PUBLIC;
				return false;
			}
			permissionLevel = EPermissionLevel.USER;
			return true;
		}

	}
}
