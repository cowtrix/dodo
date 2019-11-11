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
		[View(EPermissionLevel.USER)]
		[NoPatch]
		public string Name { get; private set; }
		public override string ResourceURL => $"{ROOT}/{Name.StripForURL()}";
		[View(EPermissionLevel.USER)]
		public GeoLocation Location { get; private set; }
		
		public ConcurrentBag<ResourceReference<User>> Members = new ConcurrentBag<ResourceReference<User>>();
		public override bool IsAuthorised(User requestOwner, HttpRequest request, out EPermissionLevel visibility)
		{
			if(requestOwner == Creator.Value)
			{
				visibility = EPermissionLevel.OWNER;
				return true;
			}
			visibility = EPermissionLevel.USER;
			return true;
		}
	}
}
