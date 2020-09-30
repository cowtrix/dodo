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

		public RoleSchema() { }

		public RoleSchema(string name, string publicDescription, string applicantQuestion, string parent) : base(name, publicDescription, parent)
		{
			ApplicantQuestion = applicantQuestion;
		}

		public override Type GetResourceType() => typeof(Role);

	}

	public class RoleFactory : DodoResourceFactory<Role, RoleSchema>
	{
	}
}
