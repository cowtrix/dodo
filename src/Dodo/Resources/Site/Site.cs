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
		public ResourceReference<Rebellion> Rebellion { get; set; }

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
		public string Description { get; private set; }

		[View(EPermissionLevel.PUBLIC)]
		public string Type { get { return GetType().FullName; } }

		public Site() : base() { }

		public Site(User creator, Passphrase passphrase, Rebellion rebellion, SiteRESTHandler.CreationSchema schema)
			: base(creator, schema.Name)
		{
			Rebellion = new ResourceReference<Rebellion>(rebellion);
			Location = schema.Location;
			Description = schema.Description;
			Facilities = new SiteFacilities();
		}

		public override string ResourceURL => $"{Rebellion.Value.ResourceURL}/{ROOT}/{Name.StripForURL()}";

		public override bool IsAuthorised(User requestOwner, Passphrase passphrase, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			if (request.MethodEnum() != EHTTPRequestType.GET)
			{
				if (Creator.Guid == requestOwner.GUID)
				{
					permissionLevel = EPermissionLevel.OWNER;
					return true;
				}
				if (Rebellion.Value.IsAdmin(requestOwner, requestOwner, passphrase))
				{
					permissionLevel = EPermissionLevel.ADMIN;
					return true;
				}
				permissionLevel = EPermissionLevel.PUBLIC;
				return false;
			}
			if(requestOwner != null)
			{
				permissionLevel = EPermissionLevel.USER;
				return true;
			}
			permissionLevel = EPermissionLevel.PUBLIC;
			return true;
		}
	}
}