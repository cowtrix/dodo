using Common;
using Common.Extensions;
using Resources.Security;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.Users;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using Resources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Resources.Location;
using Dodo.DodoResources;

namespace Dodo.LocationResources
{
	public enum EArrestRisk
	{
		High,
		Medium,
		Low,
		None,
	}

	public enum EAccessType
	{
		None,
		Free,
		Paid,
	}

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
		[View(EPermissionLevel.PUBLIC)]
		public bool DisabilityFriendly;
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType OutdoorCamping;
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType IndoorCamping;
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType Accomodation;
		[View(EPermissionLevel.PUBLIC)]
		public bool Inductions;
		[View(EPermissionLevel.PUBLIC)]
		public bool TalksAndTraining;
		[View(EPermissionLevel.PUBLIC)]
		public bool Welfare;
		[View(EPermissionLevel.PUBLIC)]
		public bool AffinityGroupFormation;
		[View(EPermissionLevel.PUBLIC)]
		public bool VolunteersNeeded;
		[View(EPermissionLevel.PUBLIC)]
		public bool FamilyFriendly;
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType Internet;
		[View(EPermissionLevel.PUBLIC)]
		public EAccessType Electricity;
	}

	[BsonDiscriminator(RootClass = true)]
	[BsonKnownTypes(
		typeof(Event),
		typeof(Site)
		)]
	public abstract class LocationResourceBase : DodoResource,
		ILocationalResource,
		IOwnedResource,
		IVideoResource,
		IPublicResource
	{
		[View(EPermissionLevel.USER)]
		[Name("Arrest Risk")]
		public EArrestRisk ArrestRisk { get; set; }
		[View(EPermissionLevel.USER)]
		public SiteFacilities Facilities { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; set; } = new GeoLocation();
		[View(EPermissionLevel.PUBLIC)]
		[Name("Public Description")]
		[Description]
		public string PublicDescription { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		public ResourceReference<GroupResource> Parent { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		[Name("Video Embed URL")]
		public string VideoEmbedURL { get; set; } = "https://www.youtube.com/embed/d4QDM_Isi24";
		[View(EPermissionLevel.ADMIN)]
		[Name("Published")]
		public bool IsPublished { get; set; }

		public LocationResourceBase () : base() {}

		public LocationResourceBase(AccessContext context, LocationResourceSchema schema) : base(context, schema)
		{
			if (schema == null)
			{
				return;
			}
			var group = ResourceUtility.GetResourceByGuid<GroupResource>(schema.Parent);
			Parent = new ResourceReference<GroupResource>(group);
			Location = schema.Location;
			PublicDescription = schema.PublicDescription;
			Facilities = new SiteFacilities();
		}
	}
}
