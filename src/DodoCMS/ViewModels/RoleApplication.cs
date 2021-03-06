// This is generated code from the DodoAOT project. DO NOT MODIFY.
using System.ComponentModel;
using Dodo.Controllers.Edit;
using Dodo.RoleApplications;
using Resources;
namespace Dodo.ViewModels
{
	public class RoleApplicationViewModel : IViewModel
	{
		public System.Type __Type => typeof(RoleApplication);
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
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public uint Revision { get; set; }
		[DisplayName("Parent")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public Resources.ResourceReference<Resources.IRESTResource> Parent { get; set; }
		public class RoleApplicationDataViewModel
		{
			[DisplayName("Messages")]
			[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
			public System.Collections.Generic.List<Dodo.RoleApplications.Message> Messages { get; set; }
		}
		[DisplayName("Data")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public RoleApplicationDataViewModel Data { get; set; }
	}
}
