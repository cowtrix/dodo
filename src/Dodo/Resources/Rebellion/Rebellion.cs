using Common;
using Common.Extensions;
using Resources.Security;
using Dodo.Sites;
using Dodo.Users;
using Dodo.WorkingGroups;
using Resources;
using Resources.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;

namespace Dodo.Rebellions
{
	[Name("Rebellion")]
	public class Rebellion : GroupResource, ILocationalResource, ITimeBoundResource
	{
		public const string ROOT = "rebellions";

		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; set; }

		[View(EPermissionLevel.PUBLIC)]
		public List<string> WorkingGroups
		{
			get
			{
				return ResourceUtility.GetManager<WorkingGroup>().Get(wg => wg.IsChildOf(this)).Select(x => x.GUID.ToString()).ToList();
			}
		}

		[View(EPermissionLevel.PUBLIC)]
		public List<string> Sites
		{
			get
			{
				return ResourceUtility.GetManager<Site>().Get(wg => wg.Parent.Guid == GUID).Select(x => x.GUID.ToString()).ToList();
			}
		}

		[View(EPermissionLevel.PUBLIC)]
		public DateTime StartDate { get; set; }

		[View(EPermissionLevel.PUBLIC)]
		public DateTime EndDate { get; set; }

		public Rebellion(AccessContext context, RebellionSchema schema) : base(context, schema)
		{
			Location = schema.Location;
			StartDate = schema.StartDate;
			EndDate = schema.EndDate;
		}

		public override bool CanContain(Type type)
		{
			if(type == typeof(WorkingGroup) || type == typeof(Site))
			{
				return true;
			}
			return false;
		}
	}
}
