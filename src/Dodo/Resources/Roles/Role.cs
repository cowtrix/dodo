using Common.Extensions;
using REST.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using REST;
using REST.Serializers;
using System.Collections.Generic;

namespace Dodo.Roles
{
	public class RoleSerializer : ResourceReferenceSerializer<Role> { }

	public class Role : DodoResource
	{
		public const string ROOT = "roles";
		[NoPatch]
		[View(EPermissionLevel.PUBLIC)]
		public ResourceReference<GroupResource> Parent { get; set; }
		public override string ResourceURL => $"{Parent.Value.ResourceURL}/{ROOT}/{Name.StripForURL()}";

		[View(EPermissionLevel.PUBLIC)]
		public string PublicDescription { get; set; }
		[View(EPermissionLevel.USER)]
		public string MemberDescription { get; set; }
		[View(EPermissionLevel.ADMIN)]
		public string AdminDescription { get; set; }

		[View(EPermissionLevel.USER)]
		public UserCollection RoleHolders;

		public Role() : base() { }

		public Role(User creator, Passphrase passphrase, GroupResource parent, RoleRESTHandler.CreationSchema schema) : base(parent.Creator, schema.Name)
		{
			Parent = new ResourceReference<GroupResource>(parent);
			PublicDescription = schema.PublicDescription;
			RoleHolders = new UserCollection(new List<ResourceReference<User>>(), creator, passphrase);
		}

		public override bool IsAuthorised(User requestOwner, Passphrase passphrase, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			if (request.MethodEnum() != EHTTPRequestType.GET)
			{
				if (Creator.Guid == requestOwner.GUID)
				{
					permissionLevel = EPermissionLevel.OWNER;
					return true;
				}
				if (Parent.Value.IsAdmin(requestOwner, requestOwner, passphrase))
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
