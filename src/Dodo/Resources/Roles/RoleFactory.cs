using Common;
using Dodo.DodoResources;
using Resources;
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
		[View(EPermissionLevel.USER)]
		[Email]
		[Name("Contact Email")]
		public string ContactEmail { get; set; }

		public RoleSchema() { }

		public RoleSchema(string name, string publicDescription, string applicantQuestion, string contactEmail, string parent) : base(name, publicDescription, parent)
		{
			ApplicantQuestion = applicantQuestion;
			ContactEmail = contactEmail;
		}

		public override Type GetResourceType() => typeof(Role);
	}

	public class RoleFactory : DodoResourceFactory<Role, RoleSchema>
	{
	}
}
