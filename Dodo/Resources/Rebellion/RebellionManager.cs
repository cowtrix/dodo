using System;
using System.Collections.Concurrent;
using System.Linq;

namespace Dodo.Rebellions
{
	public class RebellionManager : ResourceManager<Rebellion>
	{
		public Rebellion GetRebellionByName(string rebellionName)
		{
			return InternalData.Entries.SingleOrDefault(x => x.Value.RebellionName == rebellionName).Value;
		}

		public Rebellion CreateNew()
		{
			var newRebellion = new Rebellion();
			InternalData.Entries.TryAdd(newRebellion.UUID, newRebellion);
			return newRebellion;
		}
	}
}
