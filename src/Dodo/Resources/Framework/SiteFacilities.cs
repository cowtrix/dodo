using Common;
using Resources;

namespace Dodo.LocationResources
{
	public class SiteFacilities
	{
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType Toilets;
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType Bathrooms;
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType Food;
		[View(EPermissionLevel.PUBLIC)]
		public bool Kitchen;
		[Name("Disability Friendly")]
		[View(EPermissionLevel.PUBLIC)]
		public bool DisabilityFriendly;
		[Name("Outdoor Camping")]
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType OutdoorCamping;
		[Name("Indoor Camping")]
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType IndoorCamping;
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType Accomodation;
		[View(EPermissionLevel.PUBLIC)]
		public bool Inductions;
		[Name("Talks And Training")]
		[View(EPermissionLevel.PUBLIC)]
		public bool TalksAndTraining;
		[View(EPermissionLevel.PUBLIC)]
		public bool Welfare;
		[Name("Affinity Group Formation")]
		[View(EPermissionLevel.PUBLIC)]
		public bool AffinityGroupFormation;
		[Name("Volunteers Needed")]
		[View(EPermissionLevel.PUBLIC)]
		public bool VolunteersNeeded;
		[Name("Family Friendly")]
		[View(EPermissionLevel.PUBLIC)]
		public bool FamilyFriendly;
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType Internet;
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType Electricity;
	}
}
