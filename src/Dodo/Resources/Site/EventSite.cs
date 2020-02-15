using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;
using Resources;

namespace Dodo.Sites
{
	public class EventSiteSchema : TimeboundSiteSchema
	{
		public EventSiteSchema(string name, string type, Guid parent, GeoLocation location, string description, DateTime start, DateTime end) : 
			base(name, type, parent, location, description, start, end)
		{
		}
	}

	public class EventSite : Site, ITimeBoundResource
	{
		public EventSite() : base(default, default) { }

		public EventSite(AccessContext context, EventSiteSchema schema) : base(context, schema)
		{
			if(schema == null)
			{
				return;
			}
			StartDate = schema.StartDate;
			EndDate = schema.EndDate;
		}

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }
	}
}