// This is generated code from the DodoAOT project. DO NOT MODIFY.
using System.ComponentModel;
using Dodo.Controllers.Edit;
using Dodo.Roles;
using Resources;
namespace Dodo.ViewModels
{
	public class RoleViewModel : IViewModel
	{
		public System.Type __Type => typeof(Role);
		[DisplayName("Guid")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public System.Guid Guid { get; set; }
		[DisplayName("Name")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		[Resources.UserFriendlyNameAttribute()]
		public string Name { get; set; }
		[DisplayName("Slug")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		[Resources.SlugAttribute()]
		public string Slug { get; set; }
		[DisplayName("Revision")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public uint Revision { get; set; }
		[DisplayName("Parent")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public Resources.ResourceReference<Dodo.GroupResource> Parent { get; set; }
		[DisplayName("Public Description")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		[Resources.DescriptionAttribute()]
		public string PublicDescription { get; set; }
		[DisplayName("Applicant Instructions")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		[Resources.DescriptionAttribute()]
		public string ApplicantInstructions { get; set; }
		[DisplayName("Published")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public System.Boolean IsPublished { get; set; }
	}
}
