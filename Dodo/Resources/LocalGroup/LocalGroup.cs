using Common;
using Dodo.Users;
using SimpleHttpServer.REST;
using System.Collections.Concurrent;

namespace Dodo.Rebellions
{
	public class LocalGroup : RebellionResource
	{
		public LocalGroup(Rebellion owner, string name, GeoLocation location) : base(owner)
		{
			Name = name;
			Location = location;
		}
		[View(EViewVisibility.PUBLIC)]
		public string Name { get; private set; }
		public override string ResourceURL => $"lg/{Name}".StripForURL();
		[View(EViewVisibility.PUBLIC)]
		public GeoLocation Location { get; private set; }
		public ConcurrentBag<ResourceReference<User>> Members = new ConcurrentBag<ResourceReference<User>>();
	}
}
