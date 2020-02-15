﻿using Resources.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;
using Resources;

namespace Dodo.Sites
{
	public class ActionSiteSchema : TimeboundSiteSchema
	{
		public ActionSiteSchema(string name, string type, Guid parent, GeoLocation location, string description, DateTime start, DateTime end) : 
			base(name, type, parent, location, description, start, end)
		{
		}
	}

	public class ActionSite : Site, ITimeBoundResource
	{
		public ActionSite(AccessContext context, ActionSiteSchema schema) : base(context, schema)
		{
			StartDate = schema.StartDate;
			EndDate = schema.EndDate;
		}

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }
	}
}