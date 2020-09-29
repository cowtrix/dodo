using Resources.Security;
using Resources;
using System.Collections.Generic;
using Dodo.Users;
using Dodo.Roles;
using Resources.Serializers;
using Dodo.DodoResources;

namespace Dodo.RoleApplications
{

	public class RoleApplicationResourceManager : ResourceManager<RoleApplication>
	{
		protected override string MongoDBDatabaseName => nameof(RoleApplication);
	}

	public class RoleApplicationFactory : DodoResourceFactory<RoleApplication, RoleApplicationSchema> { }

	public class RoleApplicationRefSerializer : ResourceReferenceSerializer<RoleApplication> { }

	public class RoleApplicationData
	{
		public RoleApplicationData() { }
		public RoleApplicationData(AccessContext applicant, RoleApplicationSchema schema)
		{
			Messages.Add(new Message(applicant, schema.Application.Content, true));
		}

		[View(EPermissionLevel.ADMIN)]
		public string Notes { get; set; }
		[View(EPermissionLevel.ADMIN)]
		public List<Message> Messages { get; set; } = new List<Message>();
	}

	public class RoleApplication : DodoResource
	{
		public const string ROOT_URL = "roleapplication";
		public const string MESSAGE = "message";

		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM, priority: -2, customDrawer: "parentRef")]
		public ResourceReference<Role> Parent { get; set; }
		[View(EPermissionLevel.ADMIN)]
		public ApplicationStore Data;

		public RoleApplication() : base() { }

		public RoleApplication(AccessContext applicant, RoleApplicationSchema schema) : base(applicant, schema)
		{
			Parent = schema.Role.CreateRef();
			var data = new RoleApplicationData(applicant, schema);
			Data = new ApplicationStore(data, schema.ParentGroup, applicant);
		}
	}
}
