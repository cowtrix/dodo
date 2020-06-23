using Common.Extensions;
using Resources.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using Resources.Serializers;
using System.Collections.Generic;
using Resources.Location;

namespace Dodo.Roles
{
	[SearchPriority(4)]
	public class Role : DodoResource, IOwnedResource, IPublicResource, ILocationalResource
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

		public GeoLocation Location => Parent.Location;

		[View(EPermissionLevel.USER)]
		public UserCollection RoleHolders;

		public Role() : base() { }

		public Role(AccessContext context, RoleSchema schema) : base(context, schema)
		{
			var group = ResourceUtility.GetResourceByGuid<GroupResource>(schema.Parent);
			Parent = group.CreateRef();
			PublicDescription = schema.PublicDescription;
			RoleHolders = new UserCollection(new List<ResourceReference<User>>(), context);
		}
	}
}
