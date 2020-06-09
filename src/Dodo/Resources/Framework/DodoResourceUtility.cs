using Common.Extensions;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;

namespace Dodo.Resources
{
	public static class DodoResourceUtility
	{
		/// <summary>
		/// Get all resources that match a given selector.
		/// </summary>
		/// <param name="selector">A lambda function to search with.</param>
		/// <returns>An enumerable of resources that satisfy the selector</returns>
		private static IEnumerable<IRESTResource> Search(Func<IRESTResource, bool> selector, Guid? handle = null)
		{
			foreach (var rc in ResourceUtility.ResourceManagers
				.Where(rm => typeof(IPublicResource).IsAssignableFrom(rm.Key))
				.OrderByDescending(rm => rm.Key.GetCustomAttribute<SearchPriority>()?.Priority))
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

		public static IEnumerable<IRESTResource> Search(int index, int chunkSize, params ISearchFilter[] filters)
		{
			return SearchInternal<IRESTResource>(Search, index, chunkSize, filters);
		}

		public static IEnumerable<T> Search<T>(int index, int chunkSize, params ISearchFilter[] filters)
		{
			return SearchInternal<T>(ResourceUtility.GetManager(typeof(T)).Get, index, chunkSize, filters);
		}

		private static IEnumerable<T> SearchInternal<T>(
			Func<Func<IRESTResource, bool>, Guid?, IEnumerable<IRESTResource>> src, 
			int index, 
			int chunkSize, 
			ISearchFilter[] filters)
		{
			return src.Invoke(rsc => filters.All(f => f.Filter(rsc)), null)
				.OfType<IPublicResource>()
				.Where(rsc => rsc.IsPublished)
				.Transpose(x =>
				{
					Array.ForEach(filters, f => f.Mutate(x));
					return x;
				})
				.Skip(index)
				.Take(chunkSize)
				.OfType<T>();
		}
	}
}
