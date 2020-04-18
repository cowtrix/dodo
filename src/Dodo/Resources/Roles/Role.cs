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
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
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
	}
}
