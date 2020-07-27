using Common;
using Dodo.DodoResources;
using Dodo.Users;
using Resources;
using Resources.Security;
using Resources.Serializers;
using System;

namespace Dodo.Roles
{
	public class RoleSerializer : ResourceReferenceSerializer<Role> { }

	public class RoleSchema : OwnedResourceSchemaBase
	{
		[View(EPermissionLevel.USER, customDrawer:"markdown", inputHint:Role.ApplicantQuestionHint)]
		[MaxStringLength]
		[Name("Applicant Question")]
		public string ApplicantQuestion { get; set; }
		[View(EPermissionLevel.USER, inputHint:Role.ContactEmailHint)]
		[Name("Contacts")]
		[EmailUsernameList]
		public string Contacts { get; set; }

		public RoleSchema() { }

		public RoleSchema(string name, string publicDescription, string applicantQuestion, string contactEmail, string parent) : base(name, publicDescription, parent)
		{
			ApplicantQuestion = applicantQuestion;
			Contacts = contactEmail;
		}

		public override Type GetResourceType() => typeof(Role);

		public override void OnView(object requester, Passphrase passphrase)
		{
			var user = requester as User;
			Contacts = user.Slug;
		}
	}

	public class RoleFactory : DodoResourceFactory<Role, RoleSchema>
	{
	}
}
