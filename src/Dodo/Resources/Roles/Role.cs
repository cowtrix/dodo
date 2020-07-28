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
using System.Linq;

namespace Dodo.Roles
{
	public class EmailUsernameListAttribute : CustomValidatorAttribute
	{
		public static bool TryGetEmails(string value, out List<string> emails, out string error)
		{
			var userManager = ResourceUtility.GetManager<User>();
			var split = value.Split(new[] { ", ", "," }, StringSplitOptions.RemoveEmptyEntries);
			emails = new List<string>();
			foreach (var str in split)
			{
				var email = str;
				if (!ValidationExtensions.EmailIsValid(email))
				{
					// try username
					var user = userManager.GetSingle(u => u.Slug == email);
					if (user == null)
					{
						error = $"Couldn't parse value: {str}";
						return false;
					}
					email = user.PersonalData.Email;
				}
				emails.Add(email);
			}
			if(!emails.Any())
			{
				error = "Value cannot be empty";
				return false;
			}
			error = null;
			return true;
		}


		public override bool Verify(object value, out string validationError)
		{
			if(!(value is string str))
			{
				validationError = "Bad Type";
				return false;
			}
			return TryGetEmails(str, out _, out validationError);
		}
	}


	[SearchPriority(4)]
	public class Role : DodoResource, IOwnedResource, IPublicResource
	{
		const string METADATA_APPLIED = "applied";
		public const string ApplicantQuestionHint = "Here you can describe required skills, training and availabilities. All applicants will answer this prompt when applying for this role.";
		public const string ContactEmailHint = "A list of emails or usernames seperated by commas that will receive applications for this role. " + 
			"E.g. <code>mysampleemail@yahoo.com, myusername</code>";

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM, priority: -2, customDrawer:"parentRef")]
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

		[Name("Contacts")]
		[View(EPermissionLevel.ADMIN, inputHint: ContactEmailHint)]
		[EmailUsernameList]
		public string Contacts { get; set; }

		public SecureUserStore Applications = new SecureUserStore();

		public Role() : base() { }

		public Role(AccessContext context, RoleSchema schema) : base(context, schema)
		{
			Parent = schema.GetParent().CreateRef();
			PublicDescription = schema.PublicDescription;
			ApplicantQuestion = schema.ApplicantQuestion;
			Contacts = schema.Contacts;
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
			if(!EmailUsernameListAttribute.TryGetEmails(Contacts, out var emails, out error))
			{
				return false;
			}
			foreach(var contact in emails)
			{
				var proxy = EmailProxy.SetProxy(applicantEmail, contact);
				EmailUtility.SendNewRoleApplicantEmail(proxy.RemoteEmail, proxy.ProxyEmail, GetUrl(),
					Name, ApplicantQuestion, application.Content);
			}
			
			Applications.Add(context.User.CreateRef(), context.Passphrase);
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
			return Applications.IsAuthorised(context);
		}
	}
}
