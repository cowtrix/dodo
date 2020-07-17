// This is generated code from the DodoAOT project. DO NOT MODIFY.
using System.ComponentModel;
using Dodo.Controllers.Edit;
using Dodo.WorkingGroups;
using Resources;
namespace Dodo.ViewModels
{
	public class WorkingGroupViewModel : IViewModel
	{
		public System.Type __Type => typeof(WorkingGroup);
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
		[DisplayName("Public Description")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		[Resources.MaxStringLengthAttribute(2048)]
		public string PublicDescription { get; set; }
		[DisplayName("Published")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public System.Boolean IsPublished { get; set; }
		[DisplayName("MemberCount")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public int MemberCount { get; set; }
		public class AdministrationDataViewModel
		{
			[DisplayName("Administrators")]
			[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
			public System.Collections.Generic.List<Dodo.AdministratorEntry> Administrators { get; set; }
		}
		[DisplayName("AdministratorData")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public AdministrationDataViewModel AdministratorData { get; set; }
		[DisplayName("PublicKey")]
		[View(EPermissionLevel.USER, EPermissionLevel.ADMIN)]
		public string PublicKey { get; set; }
		[DisplayName("Roles")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public System.Collections.Generic.List<Resources.ResourceReference<Dodo.Roles.Role>> Roles { get; set; }
		[DisplayName("Working Groups")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public System.Collections.Generic.List<Resources.ResourceReference<Dodo.WorkingGroups.WorkingGroup>> WorkingGroups { get; set; }
		[DisplayName("Parent")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public Resources.ResourceReference<Resources.IRESTResource> Parent { get; set; }
	}
}
