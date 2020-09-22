using Resources.Security;
using Dodo.Users;
using Resources;
using System.Collections.Generic;
using Common;
using System;
using Dodo.RoleApplications;

namespace Dodo.Roles
{
	[SearchPriority(4)]
	public class Role : DodoResource, IOwnedResource, IPublicResource
	{
		const string METADATA_APPLIED = "applied";
		public const string ApplicantQuestionHint = "Here you can describe required skills, training and availabilities. All applicants will answer this prompt when applying for this role.";

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM, priority: -2, customDrawer: "parentRef")]
		public ResourceReference<IRESTResource> Parent { get; set; }

		[View(EPermissionLevel.PUBLIC, customDrawer: "markdown", inputHint: IDescribedResource.MARKDOWN_INPUT_HINT)]
		[MaxStringLength]
		[Name("Public Description")]
		[ViewDrawer("html")]
		public string PublicDescription { get; set; }

		[View(EPermissionLevel.PUBLIC, customDrawer: "markdown", inputHint: ApplicantQuestionHint)]
		[MaxStringLength]
		[Name("Applicant Question")]
		public string ApplicantQuestion { get; set; }

		[Name("Published")]
		[View(EPermissionLevel.ADMIN, customDrawer: "published", priority: -1, inputHint: IPublicResource.PublishInputHint)]
		public bool IsPublished { get; set; }

		[View(EPermissionLevel.ADMIN)]
		public Dictionary<string, ResourceReference<RoleApplication>> Applications = new Dictionary<string, ResourceReference<RoleApplication>>();

		public Role() : base() { }

		public Role(AccessContext context, RoleSchema schema) : base(context, schema)
		{
			var parentRef = schema.GetParent();
			Parent = parentRef.CreateRef();
			PublicDescription = schema.PublicDescription;
			ApplicantQuestion = schema.ApplicantQuestion;
		}

		public ResourceReference<RoleApplication> Apply(AccessContext context, ApplicationModel application, out string error)
		{
			if (context.User == null)
			{
				error = "User not logged in";
				return default;
			}
			if (!context.User.PersonalData.EmailConfirmed)
			{
				error = "User email not verified";
				return default;
			}
			if(HasApplied(context, out var id, out var app))
			{
				error = "You have already applied for this role.";
				return app;
			}
			var factory = new RoleApplicationFactory();
			app = factory.CreateTypedObject(new ResourceCreationRequest(context, new RoleApplicationSchema($"Application for {Name}", this, application)))
				.CreateRef();
			Applications.Add(id, app);
			error = null;
			return app;
		}

		public bool HasApplied(AccessContext context, out string id, out ResourceReference<RoleApplication> application)
		{
			id = SecurityExtensions.GenerateID(context.User, context.Passphrase, Guid.ToString());
			return Applications.TryGetValue(id, out application);
		}

		public override void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel, object requester, Passphrase passphrase)
		{
			var user = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).GetValue() : requester as User;
			var context = new AccessContext(user, passphrase);
			view.Add(METADATA_APPLIED, HasApplied(context, out _, out var application) ? application.Guid : default);
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}
	}
}
