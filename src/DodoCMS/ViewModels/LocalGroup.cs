// This is generated code from the DodoAOT project. DO NOT MODIFY.
using System.ComponentModel;
using Dodo.Controllers.Edit;
using Dodo.LocalGroups;
using Resources;
namespace Dodo.ViewModels
{
	public class LocalGroupViewModel : IViewModel
	{
		[DisplayName("Guid")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public System.Guid Guid { get; set; }
		[DisplayName("Name")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public string Name { get; set; }
		[DisplayName("Slug")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public string Slug { get; set; }
		[DisplayName("Revision")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public uint Revision { get; set; }
		[DisplayName("PublicDescription")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public string PublicDescription { get; set; }
		public class AdminDataViewModel
		{
			[DisplayName("Administrators")]
			[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
			public System.Collections.Generic.List<Resources.ResourceReference<Dodo.Users.User>> Administrators { get; set; }
		}
		[DisplayName("AdministratorData")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public AdminDataViewModel AdministratorData { get; set; }
		[DisplayName("MemberCount")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public int MemberCount { get; set; }
		[DisplayName("PublicKey")]
		[View(EPermissionLevel.MEMBER, EPermissionLevel.ADMIN)]
		public string PublicKey { get; set; }
		[DisplayName("IsPublished")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public System.Boolean IsPublished { get; set; }
		[DisplayName("Location")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public Resources.Location.GeoLocation Location { get; set; }
	}
}
