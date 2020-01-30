using Common.Extensions;
using REST.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using REST;
using REST.Serializers;
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

		public Role(RoleSchema schema) : base(schema)
		{
			Parent = new ResourceReference<GroupResource>(schema.Parent);
			PublicDescription = schema.PublicDescription;
			RoleHolders = new UserCollection(new List<ResourceReference<User>>(), schema.Context);
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
				if (Parent.Value.IsAdmin(context.User, context))
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
