using Common;
using Resources;
using System;
using Resources.Location;
using Dodo.LocationResources;
using System.Collections.Generic;
using System.Linq;
using Dodo.WorkingGroups;

namespace Dodo.LocalGroups
{
	[Name("Local Group")]
	[SearchPriority(1)]
	public class LocalGroup : AdministratedGroupResource, ILocationalResource
	{
		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; set; } = new GeoLocation();

		//[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		[Name("Working Groups")]
		[ViewDrawer("pubfilter")]
		public List<ResourceReference<WorkingGroup>> WorkingGroups { get; set; } = new List<ResourceReference<WorkingGroup>>();
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		[ViewDrawer("pubfilter")]
		public List<ResourceReference<Event>> Events { get; set; } = new List<ResourceReference<Event>>();

		public LocalGroup() : base() { }

		public LocalGroup(AccessContext context, LocalGroupSchema schema) : base(context, schema)
		{
			Location = schema.Location;
		}

		public override bool CanContain(Type type)
		{
			if (type == typeof(WorkingGroup) || type == typeof(Event))
			{
				return true;
			}
			return false;
		}

		public override void AddChild<T>(AccessContext context, T rsc)
		{
			if (rsc is WorkingGroup wg && wg.Parent.Guid == Guid)
			{
				if (WorkingGroups.Any(w => w.Guid == wg.Guid))
				{
					throw new Exception($"Adding duplicated child object {wg.Guid} to {Guid}");
				}
				WorkingGroups.Add(wg.CreateRef());
			}
			else if (rsc is Event e && e.Parent.Guid == Guid)
			{
				if (Events.Any(w => w.Guid == e.Guid))
				{
					throw new Exception($"Adding duplicated child object {e.Guid} to {Guid}");
				}
				Events.Add(e.CreateRef());
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
			else if (rsc is Event e && e.Parent.Guid == Guid)
			{
				return Events.Remove(e.CreateRef()) && base.RemoveChild(context, rsc);
			}
			throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
		}

		public override void OnDestroy()
		{
			/*var wgrm = ResourceUtility.GetManager<WorkingGroup>();
			foreach (var wg in WorkingGroups)
			{
				wgrm.Delete(wg.GetValue());
			}
			var erm = ResourceUtility.GetManager<Event>();
			foreach (var e in Events)
			{
				erm.Delete(e.GetValue());
			}
			base.OnDestroy();*/
		}
	}
}
