// This is generated code from the DodoAOT project. DO NOT MODIFY. This code was generated on 10/06/2020 18:03:21
using System.ComponentModel;
using Dodo.Controllers.Edit;
using Dodo.LocalGroups;
using Resources;
namespace Dodo.ViewModels
{
	public class LocalGroupViewModel
	{
		[DisplayName("Roles")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public System.Collections.Generic.IEnumerable<Dodo.Roles.Role> Roles { get; set; }
		[DisplayName("Events")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public System.Collections.Generic.IEnumerable<Dodo.LocationResources.Event> Events { get; set; }
		[DisplayName("Location")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public Resources.Location.GeoLocation Location { get; set; }
		[DisplayName("Parent")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public Resources.ResourceReference<Dodo.GroupResource> Parent { get; set; }
		[DisplayName("PublicDescription")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.String PublicDescription { get; set; }
		[DisplayName("MemberCount")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.Int32 MemberCount { get; set; }
		[DisplayName("PublicKey")]
		[View(EPermissionLevel.MEMBER, EPermissionLevel.ADMIN)]
		public System.String PublicKey { get; set; }
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
		public class AdminDataViewModel
		{
			[DisplayName("Administrators")]
			[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
			public System.Collections.Generic.List<Resources.ResourceReference<Dodo.Users.User>> Administrators { get; set; }
		}
		[DisplayName("AdministratorData")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public AdminDataViewModel AdministratorData { get; set; }
	}
}
