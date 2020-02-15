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

	public abstract class Site : DodoResource, ILocationalResource
	{
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

		public Site(AccessContext context, SiteSchema schema) : base(context, schema)
		{
			if (schema == null)
			{
				return;
			}
			Parent = new ResourceReference<GroupResource>(schema.Parent);
			Location = schema.Location;
			PublicDescription = schema.PublicDescription;
			Facilities = new SiteFacilities();
		}

		public override bool IsAuthorised(AccessContext context, EHTTPRequestType requestType, out EPermissionLevel permissionLevel)
		{
			return Parent.GetValue().IsAuthorised(context, requestType, out permissionLevel);
		}
	}
}