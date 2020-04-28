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

namespace Dodo.WorkingGroups
{
	/// <summary>
	/// A Working Group is a group of people who share a common purpose. For instance, the "Wellbeing" Working Group
	/// would take care of the wellbeing of rebels.
	/// Working Groups can have child Working Groups. A Working Group can only have a single parent Working Group.
	/// </summary>
	[Name("Working Group")]
	public class WorkingGroup : GroupResource
	{
		public WorkingGroup(AccessContext context, WorkingGroupSchema schema) : base(context, schema)
		{

		}

		[View(EPermissionLevel.PUBLIC)]
		public List<Guid> Roles = new List<Guid>();

		/// <summary>
		/// Get a list of all Working Groups that have this working group as their parent
		/// </summary>
		[View(EPermissionLevel.PUBLIC)]
		public List<Guid> WorkingGroups = new List<Guid>();

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
				if (WorkingGroups.Contains(wg.Guid))
				{
					throw new Exception($"Adding duplicated child object {wg.Guid} to {Guid}");
				}
				WorkingGroups.Add(wg.Guid);
			}
			else if (rsc is Role s && s.Parent.Guid == Guid)
			{
				if (Roles.Contains(s.Guid))
				{
					throw new Exception($"Adding duplicated child object {s.Guid} to {Guid}");
				}
				Roles.Add(s.Guid);
			}
			else
			{
				throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
			}
		}

		public override bool RemoveChild<T>(T rsc)
		{
			if (rsc is WorkingGroup wg && wg.Parent.Guid == Guid)
			{
				return WorkingGroups.Remove(wg.Guid);
			}
			else if (rsc is Role s && s.Parent.Guid == Guid)
			{
				return Roles.Remove(s.Guid);
			}
			throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
		}
	}
}
