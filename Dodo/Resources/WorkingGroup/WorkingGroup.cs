using Common;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
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
	public class WorkingGroup : GroupResource
	{
		public const string ROOT = "wg";

		public WorkingGroup(User creator, string passphrase, GroupResource parent, WorkingGroupRESTHandler.CreationSchema schema) 
			: base(creator, passphrase, parent)
		{
			Name = schema.Name;
		}

		/// <summary>
		/// The name of this Working Group
		/// </summary>
		[View(EPermissionLevel.USER)]
		[NoPatch]
		public string Name { get; private set; }

		/// <summary>
		/// The mandate of the working group - a description of its duties, what it's for
		/// </summary>
		[View(EPermissionLevel.USER)]
		public string Mandate = "";

		public override string ResourceURL => $"{Parent.Value.ResourceURL}/{ROOT}/{Name.StripForURL()}";

		[View(EPermissionLevel.USER)]
		public List<string> Roles
		{
			get
			{
				return DodoServer.ResourceManager<Role>().Get(role => role.ParentGroup.Value == this)
					.Select(x => x.GUID.ToString()).ToList();
			}
		}

		/// <summary>
		/// Get a list of all Working Groups that have this working group as their parent
		/// </summary>
		[View(EPermissionLevel.USER)]
		public List<WorkingGroup> SubGroups
		{
			get
			{
				return DodoServer.ResourceManager<WorkingGroup>().Get(wg => wg.Parent.Value == this).ToList();
			}
		}

		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel permissionLevel)
		{
			// TODO
			return Parent.Value.IsAuthorised(requestOwner, request, out permissionLevel);
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