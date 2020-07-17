using Common;
using Common.Extensions;
using Resources.Security;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using Resources.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using MongoDB.Bson.Serialization.Attributes;
using Resources.Location;

namespace Dodo.WorkingGroups
{
	/// <summary>
	/// A Working Group is a group of people who share a common purpose. For instance, the "Wellbeing" Working Group
	/// would take care of the wellbeing of rebels.
	/// Working Groups can have child Working Groups. A Working Group can only have a single parent Working Group.
	/// </summary>
	[Name("Working Group")]
	[SearchPriority(3)]
	public class WorkingGroup : AdministratedGroupResource, IOwnedResource
	{
		public WorkingGroup() : base() { }

		public WorkingGroup(AccessContext context, WorkingGroupSchema schema) : base(context, schema)
		{
			var group = ResourceUtility.GetResourceByGuid<GroupResource>(schema.Parent);
			if(group == null)
			{
				throw new Exception($"Invalid parent group ID in Working Group construction: {schema.Parent}");
			}
			Parent = group.CreateRef<IRESTResource>();
		}

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		public List<ResourceReference<Role>> Roles { get; set; } = new List<ResourceReference<Role>>();
		/// <summary>
		/// Get a list of all Working Groups that have this working group as their parent
		/// </summary>
		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM)]
		[Name("Working Groups")]
		public List<ResourceReference<WorkingGroup>> WorkingGroups { get; set; } = new List<ResourceReference<WorkingGroup>>();

		[View(EPermissionLevel.PUBLIC, EPermissionLevel.SYSTEM, priority:-2, customDrawer: "parentRef")]
		public ResourceReference<IRESTResource> Parent { get; private set; }

		public override bool CanContain(Type type)
		{
			if (type == typeof(WorkingGroup) || type == typeof(Role))
			{
				return true;
			}
			return false;
		}

		public override void AddChild<T>(T rsc)
		{
			if (rsc is WorkingGroup wg && wg.Parent.Guid == Guid)
			{
				if (WorkingGroups.Any(w => w.Guid == wg.Guid))
				{
					throw new Exception($"Adding duplicated child object {wg.Guid} to {Guid}");
				}
				WorkingGroups.Add(wg.CreateRef());
			}
			else if (rsc is Role s && s.Parent.Guid == Guid)
			{
				if (Roles.Any(w => w.Guid == s.Guid))
				{
					throw new Exception($"Adding duplicated child object {s.Guid} to {Guid}");
				}
				Roles.Add(s.CreateRef());
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
				return WorkingGroups.Remove(wg.CreateRef()) && base.RemoveChild(rsc);
			}
			else if (rsc is Role s && s.Parent.Guid == Guid)
			{
				return Roles.Remove(s.CreateRef()) && base.RemoveChild(rsc);
			}
			throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
		}
	}
}
