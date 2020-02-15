using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;

namespace Dodo.Sites
{
	public class Event : Site, ITimeBoundResource
	{
		public Event() : base(default, default) { }

		public Event(AccessContext context, TimeboundSiteSchema schema) : base(context, schema)
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