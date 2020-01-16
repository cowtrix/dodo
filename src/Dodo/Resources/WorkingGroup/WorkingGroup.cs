using Common;
using Common.Extensions;
using REST.Security;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using REST;
using REST.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.WorkingGroups
{
	public class WorkingGroupSerializer : ResourceReferenceSerializer<WorkingGroup> { }

	/// <summary>
	/// A Working Group is a group of people who share a common purpose. For instance, the "Wellbeing" Working Group
	/// would take care of the wellbeing of rebels.
	/// Working Groups can have child Working Groups. A Working Group can only have a single parent Working Group.
	/// </summary>
	[Name("Working Group")]
	public class WorkingGroup : GroupResource
	{
		public const string ROOT = "wg";

		public WorkingGroup() : base() { }

		public WorkingGroup(User creator, Passphrase passphrase, GroupResource parent, WorkingGroupRESTHandler.CreationSchema schema)
			: base(creator, passphrase, schema.Name, schema.Description, parent)
		{
		}

		public override string ResourceURL => $"{Parent.Value.ResourceURL}/{ROOT}/{Name.StripForURL()}";

		[View(EPermissionLevel.USER)]
		public List<string> Roles
		{
			get
			{
				return ResourceUtility.GetManager<Role>().Get(role => role.Parent.Guid == GUID)
					.Select(x => x.GUID.ToString()).ToList();
			}
		}

		/// <summary>
		/// Get a list of all Working Groups that have this working group as their parent
		/// </summary>
		[View(EPermissionLevel.USER)]
		public List<string> WorkingGroups
		{
			get
			{
				return ResourceUtility.GetManager<WorkingGroup>().Get(wg => wg.Parent.Value.GUID == GUID)
					.Select(wg => wg.GUID.ToString()).ToList();
			}
		}

		public override bool CanContain(Type type)
		{
			if(type == typeof(WorkingGroup) || type == typeof(Role))
			{
				return true;
			}
			return false;
		}
	}
}