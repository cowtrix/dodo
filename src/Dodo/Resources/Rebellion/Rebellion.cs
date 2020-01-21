using Common;
using Common.Extensions;
using REST.Security;
using Dodo.Sites;
using Dodo.Users;
using Dodo.WorkingGroups;
using REST;
using REST.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.Rebellions
{
	[Name("Rebellion")]
	public class Rebellion : GroupResource
	{
		public const string ROOT = "rebellions";

		[View(EPermissionLevel.USER)]
		public GeoLocation Location;

		[View(EPermissionLevel.USER)]
		public List<string> WorkingGroups
		{
			get
			{
				return ResourceUtility.GetManager<WorkingGroup>().Get(wg => wg.IsChildOf(this)).Select(x => x.GUID.ToString()).ToList();
			}
		}

		[View(EPermissionLevel.PUBLIC)]
		public DateTime StartDate { get; set; }

		[View(EPermissionLevel.PUBLIC)]
		public DateTime EndDate { get; set; }

		public Rebellion(RebellionSchema schema) : base(schema)
		{
			Location = schema.Location;
			StartDate = schema.StartDate;
			EndDate = schema.EndDate;
		}

		public override string ResourceURL
		{
			get
			{
				return $"{ROOT}/{Name.StripForURL()}";
			}
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
