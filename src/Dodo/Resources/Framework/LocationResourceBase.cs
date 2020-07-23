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
using Dodo.Users.Tokens;

namespace Dodo.LocationResources
{
	public enum EArrestRisk
	{
		High,
		Moderate,
		Low,
		None,
	}

	public enum EAccessType
	{
		None,
		Free,
		Paid,
	}

	public abstract class LocationResourceBase : GroupResource,
		ILocationalResource, IOwnedResource, IVideoResource, IPublicResource
	{
		[View(EPermissionLevel.USER)]
		[Name("Arrest Risk")]
		public EArrestRisk ArrestRisk { get; set; }
		[View(EPermissionLevel.USER, priority: 512)]
		public SiteFacilities Facilities { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; set; } = new GeoLocation();
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM, priority: -2, customDrawer: "parentRef")]
		public ResourceReference<IRESTResource> Parent { get; set; }
		[View(EPermissionLevel.PUBLIC)]
		[Name("Video Embed URL")]
		public string VideoEmbedURL { get; set; }

		public LocationResourceBase() : base() { }

		public LocationResourceBase(AccessContext context, LocationResourceSchema schema) : base(context, schema)
		{
			if (schema == null)
			{
				return;
			}
			var group = schema.GetParent();
			Parent = group.CreateRef<IRESTResource>();
			Location = schema.Location;
			PublicDescription = schema.PublicDescription;
			Facilities = new SiteFacilities();
		}

		public override Passphrase GetPrivateKey(AccessContext context)
		{
			return Parent.GetValue<ITokenResource>().GetPrivateKey(context);
		}
	}
}
