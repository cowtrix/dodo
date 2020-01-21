using Common;
using Common.Extensions;
using REST.Security;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.Users;
using Newtonsoft.Json;
using Microsoft.AspNetCore.Http;
using REST;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

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

	[ViewClass]
	public class SiteFacilities
	{
		public EAccessType Toilets;
		public EAccessType Bathrooms;
		public EAccessType Food;
		public bool Kitchen;
		public bool DisabledAccess;
		public EAccessType OutdoorCamping;
		public EAccessType IndoorCamping;
		public EAccessType Accomodation;
		public bool Inductions;
		public bool TalksAndTraining;
		public bool Welfare;
		public bool AffinityGroupFormation;
		public bool VolunteersNeeded;
		public bool FamilyFriendly;
		public EAccessType Internet;
		public EAccessType Electricity;
	}

	public abstract class Site : DodoResource
	{
		public const string ROOT = "sites";

		[View(EPermissionLevel.PUBLIC)]
		[JsonProperty]
		public ResourceReference<GroupResource> Parent { get; set; }
		[View(EPermissionLevel.USER)]
		[JsonProperty]
		public EArrestRisk ArrestRisk { get; set; }
		[View(EPermissionLevel.USER)]
		[JsonProperty]
		public SiteFacilities Facilities { get; set; }
		[JsonProperty]
		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; set; }
		/// <summary>
		/// Markdown formatted description of the site
		/// </summary>
		[JsonProperty]
		[View(EPermissionLevel.USER)]
		public string PublicDescription { get; private set; }
		[View(EPermissionLevel.PUBLIC)]
		public string Type { get { return GetType().FullName; } }

		public Site(SiteSchema schema) : base(schema)
		{
			Parent = new ResourceReference<GroupResource>(schema.Parent);
			Location = schema.Location;
			PublicDescription = schema.PublicDescription;
			Facilities = new SiteFacilities();
		}

		public override string ResourceURL => $"{Parent.Value.ResourceURL}/{ROOT}/{Name.StripForURL()}";

		public override bool IsAuthorised(AccessContext context, EHTTPRequestType requestType, out EPermissionLevel permissionLevel)
		{
			return Parent.Value.IsAuthorised(context, requestType, out permissionLevel);
		}
	}
}