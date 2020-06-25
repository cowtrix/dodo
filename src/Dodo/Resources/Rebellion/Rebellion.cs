using Common;
using Common.Extensions;
using Resources.Security;
using Dodo.LocationResources;
using Dodo.Users;
using Dodo.WorkingGroups;
using Resources;
using Resources.Serializers;
using System;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json;
using MongoDB.Bson.Serialization.Attributes;
using Resources.Location;
using Dodo.Users.Tokens;

namespace Dodo.Rebellions
{
	[Name("Rebellion")]
	[SearchPriority(0)]
	public class Rebellion : GroupResource, ILocationalResource, ITimeBoundResource, IVideoResource
	{
		public const string ROOT = "rebellions";

		[BsonElement]
		private List<Guid> m_workingGroups = new List<Guid>();
		[BsonElement]
		private List<Guid> m_sites = new List<Guid>();
		[BsonElement]
		private List<Guid> m_events = new List<Guid>();

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public IEnumerable<WorkingGroup> WorkingGroups 
		{ 
			get 
			{
				var rm = ResourceUtility.GetManager<WorkingGroup>();
				return m_workingGroups.Select(guid => rm.GetSingle(rsc => rsc.Guid == guid))
					.Where(rsc => rsc != null);
 			}
		}

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public IEnumerable<Site> Sites
		{
			get
			{
				var rm = ResourceUtility.GetManager<Site>();
				return m_sites.Select(guid => rm.GetSingle(rsc => rsc.Guid == guid))
					.Where(rsc => rsc != null);
			}
		}

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public IEnumerable<Event> Events
		{
			get
			{
				var rm = ResourceUtility.GetManager<Event>();
				return m_events.Select(guid => rm.GetSingle(rsc => rsc.Guid == guid))
					.Where(rsc => rsc != null);
			}
		}

		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime StartDate
		{
			get
			{
				return __startDate;
			}
			set
			{
				__startDate = value.ToUniversalTime();
				if (EndDate < StartDate)
				{
					EndDate = StartDate + TimeSpan.FromHours(1);
				}
			}
		}
		private DateTime __startDate;

		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		public DateTime EndDate { get { return __endDate; } set { __endDate = value.ToUniversalTime(); } }
		private DateTime __endDate;

		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; set; } = new GeoLocation();

		[View(EPermissionLevel.PUBLIC)]
		[Name("Banner Video Embed URL")]
		public string VideoEmbedURL { get; set; }

		public Rebellion() : base() { }

		public Rebellion(AccessContext context, RebellionSchema schema) : base(context, schema)
		{
			Location = schema.Location;
			StartDate = schema.StartDate;
			EndDate = schema.EndDate;
		}

		public override bool CanContain(Type type)
		{
			if (type == typeof(WorkingGroup) || type == typeof(LocationResourceBase))
			{
				return true;
			}
			return false;
		}

		public override void AddChild<T>(T rsc)
		{
			if (rsc is WorkingGroup wg && wg.Parent.Guid == Guid)
			{
				if(m_workingGroups.Contains(wg.Guid))
				{
					throw new Exception($"Adding duplicated child object {wg.Guid} to {Guid}");
				}
				m_workingGroups.Add(wg.Guid);
			}
			else if (rsc is Site s && s.Parent.Guid == Guid)
			{
				if (m_sites.Contains(s.Guid))
				{
					throw new Exception($"Adding duplicated child object {s.Guid} to {Guid}");
				}
				m_sites.Add(s.Guid);
			}
			else if (rsc is Event e && e.Parent.Guid == Guid)
			{
				if (m_events.Contains(e.Guid))
				{
					throw new Exception($"Adding duplicated child object {e.Guid} to {Guid}");
				}
				m_events.Add(e.Guid);
			}
			else
			{
				throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
			}
			base.AddChild(rsc);
		}

		public override bool RemoveChild<T>(T rsc)
		{
			if (rsc is WorkingGroup wg && wg.Parent.Guid == Guid)
			{
				return m_workingGroups.Remove(wg.Guid) && base.RemoveChild(rsc);
			}
			else if (rsc is Site s && s.Parent.Guid == Guid)
			{
				return m_sites.Remove(s.Guid) && base.RemoveChild(rsc);
			}
			else if (rsc is Event e && e.Parent.Guid == Guid)
			{
				return m_events.Remove(e.Guid) && base.RemoveChild(rsc);
			}
			throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
		}
	}
}
