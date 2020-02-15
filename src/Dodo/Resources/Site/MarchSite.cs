using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;
using Resources;

namespace Dodo.Sites
{
	public class MarchSchema : TimeboundSiteSchema
	{
		public MarchSchema(string name, string type, Guid parent, GeoLocation location, string description, DateTime start, DateTime end) : 
			base(name, type, parent, location, description, start, end)
		{
		}
	}

	public class MarchSite : Site, ITimeBoundResource
	{
		public MarchSite() : base(default, default) { }
		public MarchSite(AccessContext context, MarchSchema schema) : base(context, schema)
		{
			StartDate = schema.StartDate;
			EndDate = schema.EndDate;
		}

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }
	}
}