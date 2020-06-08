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

		[BsonElement]
		private List<Guid> m_roles = new List<Guid>();
		[BsonElement]
		private List<Guid> m_workingGroups = new List<Guid>();

		/// <summary>
		/// Get a list of all Working Groups that have this working group as their parent
		/// </summary>
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
		public IEnumerable<Role> Roles
		{
			get
			{
				var rm = ResourceUtility.GetManager<Role>();
				return m_roles.Select(guid => rm.GetSingle(rsc => rsc.Guid == guid))
					.Where(rsc => rsc != null);
			}
		}

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
				if (m_workingGroups.Contains(wg.Guid))
				{
					throw new Exception($"Adding duplicated child object {wg.Guid} to {Guid}");
				}
				m_workingGroups.Add(wg.Guid);
			}
			else if (rsc is Role s && s.Parent.Guid == Guid)
			{
				if (m_roles.Contains(s.Guid))
				{
					throw new Exception($"Adding duplicated child object {s.Guid} to {Guid}");
				}
				m_roles.Add(s.Guid);
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
			else if (rsc is Role s && s.Parent.Guid == Guid)
			{
				return m_roles.Remove(s.Guid) && base.RemoveChild(rsc);
			}
			throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
		}
	}
}
