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
using Dodo.Utility;

namespace Dodo.Roles
{
	[SearchPriority(4)]
	public class Role : DodoResource, IOwnedResource, IPublicResource
	{
		public const string ApplicantQuestionHint = "Here you can describe required skills, training and availabilities. All applicants will answer this prompt when applying for this role.";

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM, priority: -2, customDrawer:"parentRef")]
		public ResourceReference<IRESTResource> Parent { get; set; }

		[View(EPermissionLevel.PUBLIC, customDrawer: "markdown")]
		[MaxStringLength]
		[Name("Public Description")]
		public string PublicDescription { get; set; }

		[View(EPermissionLevel.PUBLIC, customDrawer: "markdown", inputHint: ApplicantQuestionHint)]
		[MaxStringLength]
		[Name("Applicant Question")]
		public string ApplicantQuestion { get; set; }

		[Name("Published")]
		[View(EPermissionLevel.ADMIN, priority: -1, inputHint: IPublicResource.PublishInputHint)]
		public bool IsPublished { get; set; }

		[Name("Contact Email")]
		[View(EPermissionLevel.ADMIN, inputHint: "This email will receive all applications for this role.")]
		public string ContactEmail { get; set; }

		public Role() : base() { }

		public Role(AccessContext context, RoleSchema schema) : base(context, schema)
		{
			var group = ResourceUtility.GetResourceByGuid<GroupResource>(schema.Parent);
			Parent = group.CreateRef<IRESTResource>();
			PublicDescription = schema.PublicDescription;
			ApplicantQuestion = schema.ApplicantQuestion;
			ContactEmail = schema.ContactEmail;
		}

		public bool Apply(AccessContext context, ApplicationModel application, out string error)
		{
			if(context.User == null)
			{
				error = "User not logged in";
				return false;
			}
			if(!context.User.PersonalData.EmailConfirmed)
			{
				error = "User email not verified";
				return false;
			}
			var applicantEmail = context.User.PersonalData.Email;
			var proxy = EmailProxy.SetProxy(applicantEmail, ContactEmail);
			var subject = $"You have a new applicant for {Name}: {context.User.Name}";
			var content = $"{subject}\n\n{ApplicantQuestion}\n\n{application.Content}";
			EmailUtility.SendEmail(proxy.RemoteEmail, "", 
				proxy.ProxyEmail, "",
				$"[{Dodo.DodoApp.PRODUCT_NAME}] {subject}",
				content, content);
			error = null;
			return true;
		}
	}
}
