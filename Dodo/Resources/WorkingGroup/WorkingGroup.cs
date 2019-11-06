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
		}
		[View(EViewVisibility.PUBLIC)]
		[NoPatch]
		public string Name { get; private set; }
		public override string ResourceURL => $"{ROOT}/{Rebellion.RebellionName.StripForURL()}/{Name.StripForURL()}";
		public ConcurrentBag<ResourceReference<User>> Members = new ConcurrentBag<ResourceReference<User>>();
	}
}
