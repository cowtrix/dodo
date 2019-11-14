using Common;
using Dodo.Rebellions;
using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Collections.Concurrent;

namespace Dodo.WorkingGroups
{
	public class WorkingGroup : RebellionResource
	{
		public const string ROOT = "workinggroups";
		public WorkingGroup(User creator, Rebellion owner, string name) : base(creator, owner)
		{
			Name = name;
			owner.WorkingGroups.Add(new ResourceReference<WorkingGroup>(this));
		}
		[View(EPermissionLevel.USER)]
		[NoPatch]
		public string Name { get; private set; }
		[View(EPermissionLevel.USER)]
		public string Mandate = "";
		public override string ResourceURL => $"{ROOT}/{Rebellion.RebellionName.StripForURL()}/{Name.StripForURL()}";
		public ConcurrentBag<ResourceReference<WorkingGroup>> SubGroups = new ConcurrentBag<ResourceReference<WorkingGroup>>();
		public ConcurrentBag<ResourceReference<User>> Members = new ConcurrentBag<ResourceReference<User>>();
	}
}
