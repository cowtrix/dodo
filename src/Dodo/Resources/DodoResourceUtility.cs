using Common.Extensions;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.Resources
{
	public static class DodoResourceUtility
	{
		/// <summary>
		/// Get all resources that match a given selector.
		/// </summary>
		/// <param name="selector">A lambda function to search with.</param>
		/// <returns>An enumerable of resources that satisfy the selector</returns>
		public static IEnumerable<IRESTResource> Get(Func<IRESTResource, bool> selector, Guid? handle = null)
		{
			foreach (var rc in ResourceUtility.ResourceManagers)
			{
				if (rc.Key == typeof(User))
				{
					continue;
				}
				foreach (var rsc in rc.Value.Get(selector))
				{
					yield return rsc;
				}
			}
		}

		public static IEnumerable<IRESTResource> Search(DistanceFilter locationFilter, DateFilter dateFilter, StringFilter stringFilter,
			int index = 0, int chunkSize = 10)
		{
			return Get(rsc =>
						locationFilter.Filter(rsc) &&
						dateFilter.Filter(rsc) &&
						stringFilter.Filter(rsc)
					).Transpose(x => locationFilter.Mutate(x))
					.Transpose(x => dateFilter.Mutate(x))
					.Transpose(x => stringFilter.Mutate(x))
					.Skip(index)
					.Take(chunkSize);
		}

		public static IEnumerable<IRESTResource> Search<T>(DistanceFilter locationFilter, DateFilter dateFilter, StringFilter stringFilter, 
			int index = 0, int chunkSize = 10)
		{
			return ResourceUtility.GetManager<T>().Get(rsc =>
						locationFilter.Filter(rsc) &&
						dateFilter.Filter(rsc) &&
						stringFilter.Filter(rsc)
					).Transpose(x => locationFilter.Mutate(x))
					.Transpose(x => dateFilter.Mutate(x))
					.Transpose(x => stringFilter.Mutate(x))
					.Skip(index)
					.Take(chunkSize);
		}
	}
}
