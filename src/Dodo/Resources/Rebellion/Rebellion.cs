using Common;
using Dodo.LocationResources;
using Dodo.WorkingGroups;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Resources.Location;
using System.ComponentModel.DataAnnotations;

namespace Dodo.Rebellions
{
	[Name("Rebellion")]
	[SearchPriority(0)]
	public class Rebellion : AdministratedGroupResource, ILocationalResource, ITimeBoundResource, IVideoResource
	{
		public const string ROOT = "rebellions";

		[View(EPermissionLevel.PUBLIC)]
		[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
		[Name("Start Date")]
		[PatchCallback(nameof(OnValueChanged))]
		[DataType(DataType.Date)]
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
		[Name("End Date")]
		[PatchCallback(nameof(OnValueChanged))]
		[DataType(DataType.Date)]
		public DateTime EndDate { get { return __endDate; } set { __endDate = value.ToUniversalTime(); } }
		private DateTime __endDate;

		[View(EPermissionLevel.PUBLIC)]
		[PatchCallback(nameof(OnValueChanged))]
		public GeoLocation Location { get; set; } = new GeoLocation();

		[View(EPermissionLevel.PUBLIC)]
		[Name("Banner Video Embed URL")]
		public string VideoEmbedURL { get; set; }

		//[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		[ViewDrawer("pubfilter")]
		[Name("Working Groups")]
		public List<ResourceReference<WorkingGroup>> WorkingGroups { get; set; } = new List<ResourceReference<WorkingGroup>>();
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		[ViewDrawer("pubfilter")]
		public List<ResourceReference<Site>> Sites { get; set; } = new List<ResourceReference<Site>>();
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		[ViewDrawer("pubfilter")]
		public List<ResourceReference<Event>> Events { get; set; } = new List<ResourceReference<Event>>();

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

		public override void AddChild<T>(AccessContext context, T rsc)
		{
			if (rsc is WorkingGroup wg && wg.Parent.Guid == Guid)
			{
				if(WorkingGroups.Any(w => w.Guid == wg.Guid))
				{
					throw new Exception($"Adding duplicated child object {wg.Guid} to {Guid}");
				}
				WorkingGroups.Add(wg.CreateRef());
			}
			else if (rsc is Site s && s.Parent.Guid == Guid)
			{
				if (Sites.Any(w => w.Guid == s.Guid))
				{
					throw new Exception($"Adding duplicated child object {s.Guid} to {Guid}");
				}
				Sites.Add(s.CreateRef());
			}
			else if (rsc is Event e && e.Parent.Guid == Guid)
			{
				if (Events.Any(w => w.Guid == e.Guid))
				{
					throw new Exception($"Adding duplicated child object {e.Guid} to {Guid}");
				}
				Events.Add(e.CreateRef());
				Events = Events.OrderBy(t => t.GetValue(false).StartDate).ToList();
			}
			else
			{
				throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
			}
			base.AddChild(context, rsc);
		}

		public override bool RemoveChild<T>(AccessContext context, T rsc)
		{
			if (rsc is WorkingGroup wg && wg.Parent.Guid == Guid)
			{
				return WorkingGroups.Remove(wg.CreateRef()) && base.RemoveChild(context, rsc);
			}
			else if (rsc is Site s && s.Parent.Guid == Guid)
			{
				return Sites.Remove(s.CreateRef()) && base.RemoveChild(context, rsc);
			}
			else if (rsc is Event e && e.Parent.Guid == Guid)
			{
				return Events.Remove(e.CreateRef()) && base.RemoveChild(context, rsc);
			}
			throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
		}

		public override void OnDestroy()
		{
			/*var wgrm = ResourceUtility.GetManager<WorkingGroup>();
			foreach(var wg in WorkingGroups)
			{
				wgrm.Delete(wg.GetValue());
			}
			var srm = ResourceUtility.GetManager<Site>();
			foreach (var s in Sites)
			{
				srm.Delete(s.GetValue());
			}
			var erm = ResourceUtility.GetManager<Event>();
			foreach (var e in Events)
			{
				erm.Delete(e.GetValue());
			}
			base.OnDestroy();*/
		}

		public override bool VerifyExplicit(out string error)
		{
			if (EndDate < StartDate)
			{
				error = "End date cannot be earlier than start date";
				return false;
			}
			return base.VerifyExplicit(out error);
		}
	}
}
