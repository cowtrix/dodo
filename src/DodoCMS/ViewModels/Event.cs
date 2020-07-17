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
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public uint Revision { get; set; }
		[DisplayName("PublicKey")]
		[View(EPermissionLevel.MEMBER, EPermissionLevel.SYSTEM)]
		public string PublicKey { get; set; }
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
			[DisplayName("Disability Friendly")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean DisabilityFriendly { get; set; }
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
			public System.Boolean Inductions { get; set; }
			[DisplayName("Talks And Training")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean TalksAndTraining { get; set; }
			[DisplayName("Welfare")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean Welfare { get; set; }
			[DisplayName("Affinity Group Formation")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean AffinityGroupFormation { get; set; }
			[DisplayName("Volunteers Needed")]
			[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
			public System.Boolean VolunteersNeeded { get; set; }
			[DisplayName("Family Friendly")]
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
		[DisplayName("Parent")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public Resources.ResourceReference<Resources.IRESTResource> Parent { get; set; }
		[DisplayName("Video Embed URL")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public string VideoEmbedURL { get; set; }
		[DisplayName("StartDate")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.DateTime StartDate { get; set; }
		[DisplayName("EndDate")]
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.ADMIN)]
		public System.DateTime EndDate { get; set; }
	}
}
