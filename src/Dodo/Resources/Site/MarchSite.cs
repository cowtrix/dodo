using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;
using Resources;
using Newtonsoft.Json;

namespace Dodo.Sites
{
	public class MarchSiteSchema : TimeboundSiteSchema
	{
		public MarchSiteSchema(string name, string type, Guid parent, GeoLocation location, string description, DateTime start, DateTime end) : 
			base(name, type, parent, location, description, start, end)
		{
		}
	}

	public class MarchSite : Site, ITimeBoundResource
	{
		public MarchSite() : base(default, default) { }
		public MarchSite(AccessContext context, MarchSiteSchema schema) : base(context, schema)
		{
			StartDate = schema.StartDate;
			EndDate = schema.EndDate;
		}

		[JsonProperty]
		[View(EPermissionLevel.PUBLIC)]
		public DateTime StartDate { get; set; }

		[JsonProperty]
		[View(EPermissionLevel.PUBLIC)]
		public DateTime EndDate { get; set; }
	}
}