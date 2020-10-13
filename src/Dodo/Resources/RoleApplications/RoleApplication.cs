using Resources.Security;
using Resources;
using System.Collections.Generic;
using Dodo.Users;
using Dodo.Roles;
using Resources.Serializers;
using Dodo.DodoResources;
using System;

namespace Dodo.RoleApplications
{

	public class RoleApplicationResourceManager : DodoResourceManager<RoleApplication>
	{
	}

	public class RoleApplicationFactory : DodoResourceFactory<RoleApplication, RoleApplicationSchema> { }

	public class RoleApplicationRefSerializer : ResourceReferenceSerializer<RoleApplication> { }

	public class RoleApplicationData
	{
		public RoleApplicationData() { }
		public RoleApplicationData(AccessContext applicant, RoleApplicationSchema schema)
		{
			Messages.Add(new Message(applicant, schema.Application.Content, true, false, Guid.NewGuid()));
		}
		[View(EPermissionLevel.ADMIN)]
		public List<Message> Messages { get; set; } = new List<Message>();
	}

	public class RoleApplication : DodoResource, IOwnedResource
	{
		public const string ROOT_URL = "roleapplication";
		public const string MESSAGE = "message";

		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM, priority: -2, customDrawer: "parentRef")]
		public ResourceReference<IRESTResource> Parent { get; set; }
		[View(EPermissionLevel.ADMIN)]
		public ApplicationStore Data;

		public RoleApplication() : base() { }

		public RoleApplication(AccessContext applicant, RoleApplicationSchema schema) : base(applicant, schema)
		{
			Parent = schema.Role.CreateRef<IRESTResource>();
			var data = new RoleApplicationData(applicant, schema);
			Data = new ApplicationStore(data, schema.ParentGroup, applicant);
		}
	}
}
