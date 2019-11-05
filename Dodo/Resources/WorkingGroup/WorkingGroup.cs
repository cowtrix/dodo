using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Collections.Concurrent;

namespace Dodo.Rebellions
{
	public class WorkingGroup : RebellionResource
	{
		public WorkingGroup(Rebellion owner, string name) : base(owner)
		{
			Name = name;
		}
		[View(EViewVisibility.PUBLIC)]
		public string Name { get; private set; }
		public override string ResourceURL => $"{Rebellion.ResourceURL}/wg/{Name}".StripForURL();
		public ConcurrentBag<ResourceReference<User>> Members = new ConcurrentBag<ResourceReference<User>>();
	}
}
