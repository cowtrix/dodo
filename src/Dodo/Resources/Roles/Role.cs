using Resources.Security;
using Dodo.Users;
using Resources;
using System.Collections.Generic;
using Common;
using System;
using Dodo.RoleApplications;
using System.Linq;
using System.Threading.Tasks;

namespace Dodo.Roles
{
	[SearchPriority(4)]
	public class Role : DodoResource, IOwnedResource, IPublicResource, IParentResource
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

		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM, customDrawer:"null")]
		public Dictionary<string, ResourceReference<RoleApplication>> Applications = new Dictionary<string, ResourceReference<RoleApplication>>();

		public Role() : base() { }

		public Role(AccessContext context, RoleSchema schema) : base(context, schema)
		{
			var parentRef = schema.GetParent();
			Parent = parentRef.CreateRef();
			PublicDescription = schema.PublicDescription;
			ApplicantQuestion = schema.ApplicantQuestion;
		}

		private string GetID(AccessContext context) => SecurityExtensions.GenerateID(context.User, context.Passphrase, Guid.ToString());

		public bool HasApplied(AccessContext context, out string id, out ResourceReference<RoleApplication> application)
		{
			id = GetID(context);
			return Applications.TryGetValue(id, out application);
		}

		public override void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel, object requester, Passphrase passphrase)
		{
			var user = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).GetValue() : requester as User;
			var context = new AccessContext(user, passphrase);
			view.Add(METADATA_APPLIED, HasApplied(context, out _, out var application) ? application.Guid : default);
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}

		public bool CanContain(Type type) => type == typeof(RoleApplication);

		void IParentResource.AddChild<T>(AccessContext context, T rsc)
		{
			if(!(rsc is RoleApplication app))
			{
				throw new InvalidCastException($"Resource must be a RoleApplication");
			}
			var id = GetID(context);
			Applications.Add(id, app.CreateRef());
		}

		bool IParentResource.RemoveChild<T>(AccessContext context, T rsc)
		{
			if (!(rsc is RoleApplication app))
			{
				throw new InvalidCastException($"Resource must be a RoleApplication");
			}
			var contained = Applications.FirstOrDefault(a => a.Value.Guid == rsc.Guid);
			if(string.IsNullOrEmpty(contained.Key))
			{
				return false;
			}
			return Applications.Remove(contained.Key);
		}
	}
}
