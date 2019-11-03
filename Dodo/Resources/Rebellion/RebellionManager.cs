﻿using Dodo.Users;
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
	}
}
