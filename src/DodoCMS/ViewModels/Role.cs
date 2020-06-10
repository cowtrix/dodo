// This is generated code from the DodoAOT project. DO NOT MODIFY. This code was generated on 10/06/2020 18:03:21
using System.ComponentModel;
using Dodo.Controllers.Edit;
using Dodo.Roles;
using Resources;
namespace Dodo.ViewModels
{
	public class RoleViewModel
	{
		[DisplayName("Parent")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public Resources.ResourceReference<Dodo.GroupResource> Parent { get; set; }
		[DisplayName("PublicDescription")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.String PublicDescription { get; set; }
		[DisplayName("MemberDescription")]
		[View(EPermissionLevel.MEMBER, EPermissionLevel.ADMIN)]
		public System.String MemberDescription { get; set; }
		[DisplayName("AdminDescription")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public System.String AdminDescription { get; set; }
		[DisplayName("IsPublished")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public System.Boolean IsPublished { get; set; }
		[DisplayName("Creator")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public System.String Creator { get; set; }
		[DisplayName("Guid")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public System.Guid Guid { get; set; }
		[DisplayName("Name")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.String Name { get; set; }
		[DisplayName("Slug")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.String Slug { get; set; }
		[DisplayName("Revision")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public System.UInt32 Revision { get; set; }
		[DisplayName("RoleHolders")]
		[View(EPermissionLevel.USER, EPermissionLevel.ADMIN)]
		public System.Collections.Generic.List<Resources.ResourceReference<Dodo.Users.User>> RoleHolders { get; set; }
	}
}
