// This is generated code from the DodoAOT project. DO NOT MODIFY.
using System.ComponentModel;
using Dodo.Controllers.Edit;
using Dodo.LocationResources;
using Resources;
namespace Dodo.ViewModels
{
	public class EventViewModel : IViewModel
	{
		public System.Type __Type => typeof(Event);
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
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public uint Revision { get; set; }
		[DisplayName("Public Description")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		[Resources.MaxStringLengthAttribute((int)2048)]
		public string PublicDescription { get; set; }
		[DisplayName("Published")]
		[View(EPermissionLevel.ADMIN, EPermissionLevel.ADMIN)]
		public bool IsPublished { get; set; }
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
		public Resources.Security.Passphrase PublicKey { get; set; }
		[DisplayName("Arrest Risk")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
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
			public bool Kitchen { get; set; }
			[DisplayName("Disability Friendly")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public bool DisabilityFriendly { get; set; }
			[DisplayName("Outdoor Camping")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType OutdoorCamping { get; set; }
			[DisplayName("Indoor Camping")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType IndoorCamping { get; set; }
			[DisplayName("Accomodation")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType Accomodation { get; set; }
			[DisplayName("Inductions")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public bool Inductions { get; set; }
			[DisplayName("Talks And Training")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public bool TalksAndTraining { get; set; }
			[DisplayName("Welfare")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public bool Welfare { get; set; }
			[DisplayName("Affinity Group Formation")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public bool AffinityGroupFormation { get; set; }
			[DisplayName("Volunteers Needed")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public bool VolunteersNeeded { get; set; }
			[DisplayName("Family Friendly")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public bool FamilyFriendly { get; set; }
			[DisplayName("Internet")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType Internet { get; set; }
			[DisplayName("Electricity")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public Dodo.LocationResources.EAccessType Electricity { get; set; }
		}
		[DisplayName("Facilities")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public SiteFacilitiesViewModel Facilities { get; set; }
		[DisplayName("Video Embed URL")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public string VideoEmbedURL { get; set; }
		[DisplayName("Location")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public Resources.Location.GeoLocation Location { get; set; }
		[DisplayName("Parent")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public Resources.ResourceReference<Resources.IRESTResource> Parent { get; set; }
		[DisplayName("Start Date")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.DateTime StartDate { get; set; }
		[DisplayName("End Date")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.DateTime EndDate { get; set; }
	}
}
