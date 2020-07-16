using Common.Extensions;
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
		public string ApplicantQuestion { get; set; }
		public RoleSchema() { }

		public RoleSchema(string name, string publicDescription, string applicantQuestion, Guid parent) : base(name, publicDescription, parent)
		{
			ApplicantQuestion = applicantQuestion;
		}
	}

	public class RoleFactory : DodoResourceFactory<Role, RoleSchema>
	{
	}
}
