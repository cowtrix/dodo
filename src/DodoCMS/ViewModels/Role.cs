// This is generated code from the DodoAOT project. DO NOT MODIFY. This code was generated on 11/06/2020 00:49:27
using System.ComponentModel;
using Dodo.Controllers.Edit;
using Dodo.Roles;
using Resources;
namespace Dodo.ViewModels
{
	public class RoleViewModel : IViewModel
	{
		[DisplayName("Name")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public string Name { get; set; }
		[DisplayName("Slug")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public string Slug { get; set; }
		[DisplayName("Revision")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public uint Revision { get; set; }
		[DisplayName("Parent")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public Resources.ResourceReference<Dodo.GroupResource> Parent { get; set; }
		[DisplayName("PublicDescription")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public string PublicDescription { get; set; }
		[DisplayName("MemberDescription")]
		[View(EPermissionLevel.MEMBER, EPermissionLevel.ADMIN)]
		public string MemberDescription { get; set; }
		[DisplayName("AdminDescription")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public string AdminDescription { get; set; }
		[DisplayName("IsPublished")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public System.Boolean IsPublished { get; set; }
		[DisplayName("RoleHolders")]
		[View(EPermissionLevel.USER, EPermissionLevel.ADMIN)]
		public System.Collections.Generic.List<Resources.ResourceReference<Dodo.Users.User>> RoleHolders { get; set; }
	}
}
