using Common.Extensions;
using Resources.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using Resources.Serializers;
using System.Collections.Generic;

namespace Dodo.Roles
{
	[SearchPriority(4)]
	public class Role : DodoResource, IOwnedResource, IPublicResource
	{
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public ResourceReference<GroupResource> Parent { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		[Description]
		public string PublicDescription { get; set; }
		[View(EPermissionLevel.MEMBER)]
		public string MemberDescription { get; set; }
		[View(EPermissionLevel.ADMIN)]
		public string AdminDescription { get; set; }
		[View(EPermissionLevel.ADMIN)]
		public bool IsPublished { get; set; }

		[View(EPermissionLevel.USER)]
		public UserCollection RoleHolders;

		public Role() : base() { }

		public Role(AccessContext context, RoleSchema schema) : base(context, schema)
		{
			var group = ResourceUtility.GetResourceByGuid<GroupResource>(schema.Parent);
			Parent = new ResourceReference<GroupResource>(group);
			PublicDescription = schema.PublicDescription;
			RoleHolders = new UserCollection(new List<ResourceReference<User>>(), context);
		}
	}
}
