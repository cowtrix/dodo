// This is generated code from the DodoAOT project. DO NOT MODIFY.
using System.ComponentModel;
using Dodo.Controllers.Edit;
using Dodo.Rebellions;
using Resources;
namespace Dodo.ViewModels
{
	public class RebellionViewModel : IViewModel
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
		[DisplayName("Public Description")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public string PublicDescription { get; set; }
		public class AdministrationDataViewModel
		{
			[DisplayName("Administrators")]
			[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
			public System.Collections.Generic.List<Dodo.AdministratorEntry> Administrators { get; set; }
		}
		[DisplayName("Administrator Data")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public AdministrationDataViewModel AdministratorData { get; set; }
		[DisplayName("MemberCount")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public int MemberCount { get; set; }
		[DisplayName("PublicKey")]
		[View(EPermissionLevel.MEMBER, EPermissionLevel.ADMIN)]
		public string PublicKey { get; set; }
		[DisplayName("IsPublished")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public System.Boolean IsPublished { get; set; }
		[DisplayName("StartDate")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.DateTime StartDate { get; set; }
		[DisplayName("EndDate")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.DateTime EndDate { get; set; }
		[DisplayName("Location")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public Resources.Location.GeoLocation Location { get; set; }
	}
}
