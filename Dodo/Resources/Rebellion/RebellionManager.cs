using Common;
using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.Rebellions
{
	public class RebellionManager : ResourceManager<Rebellion>
	{
		public override string BackupPath => "rebellions";

		public Rebellion GetRebellionByName(string rebellionName)
		{
			return InternalData.Entries.SingleOrDefault(x => x.Value.RebellionName == rebellionName).Value;
		}

		public Rebellion CreateNew(User creator, string name, GeoLocation location)
		{
			var newRebellion = new Rebellion(creator, name, location);
			InternalData.Entries.TryAdd(newRebellion.UUID, newRebellion);
			return newRebellion;
		}

		protected override bool IsAuthorised(HttpRequest request, Rebellion resource, out EViewVisibility visibility)
		{
			var requestOwner = DodoRESTServer.GetRequestOwner(request);
			if (requestOwner == resource.Creator.Value)
			{
				visibility = EViewVisibility.OWNER;
				return true;
			}
			visibility = EViewVisibility.PUBLIC;
			return request.Method == EHTTPRequestType.GET;
		}
	}
}
