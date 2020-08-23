using Resources.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using Resources.Serializers;
using System.Collections.Generic;
using Resources.Location;
using Common;
using MongoDB.Bson.Serialization.Attributes;
using Dodo.Utility;
using Dodo.Users.Tokens;
using Newtonsoft.Json;
using System;

namespace Dodo.Roles
{
	public class RoleApplicationData
	{
		public class Message
		{
			public DateTime Timestamp { get; set; }
			public string Content { get; set; }
		}
		public ResourceReference<User> Applicant { get; set; }
		public string Notes { get; set; }
		public List<Message> Messages { get; set; } = new List<Message>();

		public RoleApplicationData(User user)
		{
			Applicant = user.CreateRef();
		}
	}

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
		public Dictionary<string, GroupUserEncryptedStore> Applications { get; set; } = new Dictionary<string, GroupUserEncryptedStore>();

		public Role() : base() { }

		public Role(AccessContext context, RoleSchema schema) : base(context, schema)
		{
			var parentRef = schema.GetParent();
			Parent = parentRef.CreateRef();
			PublicDescription = schema.PublicDescription;
			ApplicantQuestion = schema.ApplicantQuestion;
		}

		public bool Apply(AccessContext context, ApplicationModel application, out string error)
		{
			if (context.User == null)
			{
				error = "User not logged in";
				return false;
			}
			if (!context.User.PersonalData.EmailConfirmed)
			{
				error = "User email not verified";
				return false;
			}
			var app = new GroupUserEncryptedStore(new RoleApplicationData(context.User), Parent.GetValue<IAsymmCapableResource>(), context);
			var id = SecurityExtensions.GenerateID(context.User, context.Passphrase, Guid.ToString());
			Applications.Add(id, app);
			error = null;
			return true;
		}

		public override void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel, object requester, Passphrase passphrase)
		{
			var user = requester is ResourceReference<User> ? ((ResourceReference<User>)requester).GetValue() : requester as User;
			var context = new AccessContext(user, passphrase);
			view.Add(METADATA_APPLIED, HasApplied(context));
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
		}

		private bool HasApplied(AccessContext context)
		{
			var id = SecurityExtensions.GenerateID(context.User, context.Passphrase, Guid.ToString());
			return Applications.ContainsKey(id);
		}
	}
}
