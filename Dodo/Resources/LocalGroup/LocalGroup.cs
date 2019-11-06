using Common;
using Dodo.Users;
using Dodo.Rebellions;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System.Collections.Concurrent;

namespace Dodo.LocalGroups
{
	public class LocalGroup : DodoResource
	{
		public const string ROOT = "localgroups";
		public LocalGroup(User owner, string name, GeoLocation location) : base(owner)
		{
			Name = name;
			Location = location;
		}
		[View(EViewVisibility.PUBLIC)]
		[NoPatch]
		public string Name { get; private set; }
		public override string ResourceURL => $"{ROOT}/{Name.StripForURL()}";
		[View(EViewVisibility.PUBLIC)]
		public GeoLocation Location { get; private set; }
		public ConcurrentBag<ResourceReference<User>> Members = new ConcurrentBag<ResourceReference<User>>();
		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EViewVisibility visibility)
		{
			if(requestOwner == Creator.Value)
			{
				visibility = EViewVisibility.OWNER;
				return true;
			}
			visibility = EViewVisibility.PUBLIC;
			return true;
		}
	}
}
