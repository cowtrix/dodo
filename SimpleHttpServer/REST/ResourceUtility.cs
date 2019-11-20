using Common;
using SimpleHttpServer.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleHttpServer.REST
{

	public static class ResourceUtility
	{
		private static ConcurrentDictionary<IResourceManager, bool> ResourceManagers = new ConcurrentDictionary<IResourceManager, bool>();
		public static void Register(IResourceManager manager)
		{
			ResourceManagers[manager] = true;
		}

		public static T GetResourceByGuid<T>(this Guid guid) where T : class, IRESTResource
		{
			var rm = GetManagerForResource(guid);
			return (T)rm?.GetSingle(resource => resource.GUID == guid);
		}

		public static IRESTResource GetResourceByGuid(this Guid guid)
		{
			var rm = GetManagerForResource(guid);
			return rm?.GetSingle(resource => resource.GUID == guid);
		}

		public static IResourceManager GetManagerForResource(this Guid guid)
		{
			return ResourceManagers.SingleOrDefault(x => x.Key.Get(resource => resource.GUID == guid).Any()).Key;
		}

		public static IResourceManager GetManagerForResource(this Resource resource)
		{
			return GetManagerForResource(resource.GUID);
		}

		public static bool IsAuthorized<T>(HttpRequest request, T resource, out EPermissionLevel visibility) where T:Resource
		{
			var rm = GetManagerForResource(resource.GUID);
			if(rm == null)
			{
				throw new Exception($"Orphan resource {resource} with guid {resource.GUID}");
			}
			return rm.IsAuthorised(request, resource, out visibility);
		}
	}
}
