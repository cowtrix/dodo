// This is generated code from the DodoAOT project. DO NOT MODIFY. This code was generated on 11/06/2020 00:49:27
using System.ComponentModel;
using Dodo.Controllers.Edit;
using Dodo.LocationResources;
using Resources;
namespace Dodo.ViewModels
{
	public class EventViewModel : IViewModel
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
		[DisplayName("Arrest Risk")]
		[View(EPermissionLevel.USER, EPermissionLevel.ADMIN)]
		public Dodo.LocationResources.EArrestRisk ArrestRisk { get; set; }
		public class SiteFacilitiesViewModel
		{
			[DisplayName("Toilets")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType Toilets { get; set; }
			[DisplayName("Bathrooms")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType Bathrooms { get; set; }
			[DisplayName("Food")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType Food { get; set; }
			[DisplayName("Kitchen")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean Kitchen { get; set; }
			[DisplayName("DisabilityFriendly")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean DisabilityFriendly { get; set; }
			[DisplayName("OutdoorCamping")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType OutdoorCamping { get; set; }
			[DisplayName("IndoorCamping")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType IndoorCamping { get; set; }
			[DisplayName("Accomodation")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType Accomodation { get; set; }
			[DisplayName("Inductions")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean Inductions { get; set; }
			[DisplayName("TalksAndTraining")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean TalksAndTraining { get; set; }
			[DisplayName("Welfare")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean Welfare { get; set; }
			[DisplayName("AffinityGroupFormation")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean AffinityGroupFormation { get; set; }
			[DisplayName("VolunteersNeeded")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean VolunteersNeeded { get; set; }
			[DisplayName("FamilyFriendly")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean FamilyFriendly { get; set; }
			[DisplayName("Internet")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType Internet { get; set; }
			[DisplayName("Electricity")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType Electricity { get; set; }
		}
		[DisplayName("Facilities")]
		[View(EPermissionLevel.USER, EPermissionLevel.ADMIN)]
		public SiteFacilitiesViewModel Facilities { get; set; }
		[DisplayName("Location")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public Resources.Location.GeoLocation Location { get; set; }
		[DisplayName("Public Description")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public string PublicDescription { get; set; }
		[DisplayName("Parent")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public Resources.ResourceReference<Dodo.GroupResource> Parent { get; set; }
		[DisplayName("Video Embed URL")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public string VideoEmbedURL { get; set; }
		[DisplayName("Published")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public System.Boolean IsPublished { get; set; }
		[DisplayName("StartDate")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.DateTime StartDate { get; set; }
		[DisplayName("EndDate")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.DateTime EndDate { get; set; }
	}
}
