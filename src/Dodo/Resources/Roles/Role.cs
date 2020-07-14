using Common.Extensions;
using Resources.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using Resources.Serializers;
using System.Collections.Generic;
using Resources.Location;
using Common;
using MongoDB.Bson.Serialization.Attributes;
using System;

namespace Dodo.Roles
{
	[SearchPriority(4)]
	public class Role : DodoResource, IOwnedResource, IPublicResource, ILocationalResource
	{
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM, priority: -2, customDrawer:"parentRef")]
		public ResourceReference<IRESTResource> Parent { get; set; }

		[View(EPermissionLevel.PUBLIC, customDrawer: "markdown")]
		[Description]
		[Name("Public Description")]
		public string PublicDescription { get; set; }

		[View(EPermissionLevel.PUBLIC, customDrawer: "markdown", inputHint:"All applicants will answer this prompt when applying.")]
		[Description]
		[Name("Applicant Instructions")]
		public string ApplicantInstructions { get; set; }

		[Name("Published")]
		[View(EPermissionLevel.ADMIN, priority: -1, inputHint: IPublicResource.PublishInputHint)]
		public bool IsPublished { get; set; }

		public GeoLocation Location => Parent.Location;

		public Role() : base() { }

		public Role(AccessContext context, RoleSchema schema) : base(context, schema)
		{
			var group = ResourceUtility.GetResourceByGuid<GroupResource>(schema.Parent);
			Parent = group.CreateRef<IRESTResource>();
			PublicDescription = schema.PublicDescription;
		}

		public bool Apply(AccessContext context, ApplicationModel application)
		{
			return true;
		}
	}
}
