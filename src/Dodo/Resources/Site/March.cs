using REST.Security;
using Dodo.Rebellions;
using Dodo.Users;
using Common;
using System;

namespace Dodo.Sites
{
	public class MarchSchema : TimeboundSiteSchema
	{
	}

	public class March : Site, ITimeBoundResource
	{
		public March(AccessContext context, TimeboundSiteSchema schema) : base(context, schema)
		{
			StartDate = schema.StartDate;
			EndDate = schema.EndDate;
		}

		public DateTime StartDate { get; set; }

		public DateTime EndDate { get; set; }
	}
}