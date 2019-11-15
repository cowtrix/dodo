using Common;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
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
	public class WorkingGroup : RebellionResource
	{
		public const string ROOT = "workinggroups";
		public WorkingGroup(User creator, Rebellion owner, WorkingGroup parentGroup, string name) : base(creator, owner)
		{
			Name = name;
			owner.WorkingGroups.Add(new ResourceReference<WorkingGroup>(this));
			ParentGroup = new ResourceReference<WorkingGroup>(parentGroup);
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
		[View(EPermissionLevel.USER, EPermissionLevel.ADMIN)]
		public string Mandate = "";

		public override string ResourceURL => $"{ROOT}/{Rebellion.RebellionName.StripForURL()}/{Name.StripForURL()}";

		/// <summary>
		/// An optional parent group that contains this working group
		/// </summary>
		[View(EPermissionLevel.USER)]
		public ResourceReference<WorkingGroup> ParentGroup;

		/// <summary>
		/// Roles are linked to users and are
		/// </summary>
		[View(EPermissionLevel.USER)]
		public List<Role> Roles
		{
			get
			{
				return DodoServer.ResourceManager<Role>().Get(role => role.ParentGroup.Value == this).ToList();
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
				return DodoServer.ResourceManager<WorkingGroup>().Get(wg => wg.ParentGroup.Value == this).ToList();
			}
		}
	}
}