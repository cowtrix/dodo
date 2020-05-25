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
using Dodo.Resources;

namespace Dodo.Sites
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
		public bool DisabledAccess;
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
		typeof(EventSite),
		typeof(PermanentSite),
		typeof(MarchSite)
		)]
	public abstract class Site : DodoResource,
		ILocationalResource,
		IOwnedResource,
		IMediaResource
	{
		private const string METADATA_SITE_TYPE = "site_type";

		[View(EPermissionLevel.USER)]
		public EArrestRisk ArrestRisk { get; set; }
		[View(EPermissionLevel.USER)]
		public SiteFacilities Facilities { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		public string PublicDescription { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		public ResourceReference<GroupResource> Parent { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		public string VideoEmbedURL { get; set; } = "https://www.youtube.com/watch?v=d4QDM_Isi24";
		[View(EPermissionLevel.PUBLIC)]
		public string PhotoEmbedURL { get; set; }

		public Site () : base() {}

		public Site(AccessContext context, SiteSchema schema) : base(context, schema)
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

		public override void AppendMetadata(Dictionary<string, object> view, EPermissionLevel permissionLevel, object requester, Passphrase passphrase)
		{
			base.AppendMetadata(view, permissionLevel, requester, passphrase);
			view[METADATA_TYPE] = typeof(Site).Name.ToLowerInvariant();
			view.Add(METADATA_SITE_TYPE, GetType().Name.ToLowerInvariant());
		}
	}
}
