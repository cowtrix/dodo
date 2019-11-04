using Common;
using SimpleHttpServer.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleHttpServer.REST
{
	public struct ResourceReferece<T> where T : Resource
	{
		public Guid Guid;
		public T Value { get { return ResourceUtility.GetResourceByGuid<T>(Guid); } }
		public ResourceReferece(T resource)
		{
			Guid = resource.UUID;
		}
	}

	public static class ResourceUtility
	{
		private static ConcurrentDictionary<IResourceManager, bool> ResourceManagers = new ConcurrentDictionary<IResourceManager, bool>();
		public static void Register(IResourceManager manager)
		{
			ResourceManagers[manager] = true;
		}

		public static string StripForURL(this string s)
		{
			return System.Net.WebUtility.UrlEncode(Regex.Replace(s.ToLower(), @"\s+", ""));
		}

		public static T GetResourceByGuid<T>(this Guid guid) where T : Resource
		{
			return ResourceManagers.Where(x => x.Key.GetType().GenericTypeArguments.FirstOrDefault() == typeof(T))
				.Select(x => x.Key as ResourceManager<T>)
				.Select(x => x.GetSingle(resource => resource.UUID == guid))
				.SingleOrDefault();
		}

		public static Resource GetResourceByGuid(this Guid guid)
		{
			return ResourceManagers.Select(x => x.Key.Get(resource => resource.UUID == guid))
				.ConcatenateCollection()
				.SingleOrDefault() as Resource;
		}

		public static IResourceManager GetManagerForResource(this Guid guid)
		{
			return ResourceManagers.SingleOrDefault(x => x.Key.Get(resource => resource.UUID == guid).Any()).Key;
		}

		public static bool IsAuthorized<T>(HttpRequest request, T resource) where T:Resource
		{
			var rm = GetManagerForResource(resource.UUID);
			if(rm == null)
			{
				throw new Exception($"Orphan resource {resource} with guid {resource.UUID}");
			}
			return rm.IsAuthorised(request, resource);
		}
	}
}
