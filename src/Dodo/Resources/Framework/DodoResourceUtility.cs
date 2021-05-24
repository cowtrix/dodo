using Common.Extensions;
using Dodo.Users;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dodo.DodoResources
{
	public static class DodoResourceUtility
	{
		/// <summary>
		/// Get all resources that match a given selector.
		/// </summary>
		/// <param name="selector">A lambda function to search with.</param>
		/// <returns>An enumerable of resources that satisfy the selector</returns>
		private static IEnumerable<IRESTResource> Search(Func<IRESTResource, bool> selector, Guid? handle = null, bool ensureLatest = false)
		{
			foreach (var rc in ResourceUtility.ResourceManagers
				.Where(rm => typeof(IPublicResource).IsAssignableFrom(rm.Key)))
			{
				if (rc.Key == typeof(User))
				{
					continue;
				}
				foreach (var rsc in rc.Value.Get(selector, handle, ensureLatest))
				{
					yield return rsc;
				}
			}
		}

		public static IEnumerable<IRESTResource> Search(int index, int chunkSize, bool ensureLatest, params ISearchFilter[] filters)
		{
			return SearchInternal<IRESTResource>(Search, index, chunkSize, ensureLatest, filters);
		}

		public static IEnumerable<T> Search<T>(int index, int chunkSize, bool ensureLatest, params ISearchFilter[] filters)
		{
			return SearchInternal<T>(ResourceUtility.GetManager(typeof(T)).Get, index, chunkSize, ensureLatest, filters);
		}

		private static IEnumerable<T> SearchInternal<T>(
			Func<Func<IRESTResource, bool>, Guid?, bool, IEnumerable<IRESTResource>> src,
			int index,
			int chunkSize,
			bool ensureLatest,
			ISearchFilter[] filters)
		{
			return src
				.Invoke(rsc => filters.All(f => f.Filter((IPublicResource)rsc)), null, ensureLatest)
				.OfType<IPublicResource>()
				.Where(rsc => rsc.IsPublished && !rsc.IsHidden())
				.Transpose(rscList =>
				{
					foreach(var filter in filters)
					{
						rscList = filter.Mutate(rscList);
					}
					return rscList;
				})
				.Skip(index)
				.Take(chunkSize)
				.OfType<T>();
		}
	}
}
