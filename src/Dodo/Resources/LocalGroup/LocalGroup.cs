using Common;
using Dodo.Users;
using Resources;
using System;
using Dodo.Roles;
using Common.Extensions;
using Resources.Security;
using Resources.Serializers;
using Resources.Location;
using Dodo.LocationResources;
using MongoDB.Bson.Serialization.Attributes;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.LocalGroups
{
	[Name("Local Group")]
	public class LocalGroup : GroupResource, ILocationalResource
	{
		[BsonElement]
		private List<Guid> m_events = new List<Guid>();
		[BsonElement]
		private List<Guid> m_roles = new List<Guid>();

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

		public LocalGroup(AccessContext context, LocalGroupSchema schema) : base(context, schema)
		{
			Location = schema.Location;
		}

		[View(EPermissionLevel.PUBLIC)]
		public GeoLocation Location { get; private set; }

		public override bool CanContain(Type type)
		{
			if (type == typeof(Role) || type == typeof(Event))
			{
				return true;
			}
			return false;
		}

		public override void AddChild<T>(T rsc)
		{
			if (rsc is Role role && role.Parent.Guid == Guid)
			{
				if (m_roles.Contains(role.Guid))
				{
					throw new Exception($"Adding duplicated child object {role.Guid} to {Guid}");
				}
				m_roles.Add(role.Guid);
			}
			else if (rsc is Event evt && evt.Parent.Guid == Guid)
			{
				if (m_events.Contains(evt.Guid))
				{
					throw new Exception($"Adding duplicated child object {evt.Guid} to {Guid}");
				}
				m_events.Add(evt.Guid);
			}
			else
			{
				throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
			}
		}

		public override bool RemoveChild<T>(T rsc)
		{
			if (rsc is Role role && role.Parent.Guid == Guid)
			{
				return m_roles.Remove(role.Guid);
			}
			else if (rsc is Event s && s.Parent.Guid == Guid)
			{
				return m_events.Remove(s.Guid);
			}
			throw new Exception($"Unsupported sub-resource type {rsc.GetType()}");
		}
	}
}
