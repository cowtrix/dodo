using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using System;
using System.Collections.Generic;

namespace Resources
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
				foreach(var rsc in rc.Value.Get(selector))
				{
					yield return rsc;
				}
			}
		}
	}
}
