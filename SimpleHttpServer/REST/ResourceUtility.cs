using Common;
using SimpleHttpServer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleHttpServer.REST
{
	public struct ResourceReference<T> where T : Resource
	{
		public Guid Guid;
		public T Value { get { return ResourceUtility.GetResourceByGuid<T>(Guid); } }
		public ResourceReference(T resource)
		{
			Guid = resource != null ? resource.GUID : default;
		}

		public override bool Equals(object obj)
		{
			return obj is ResourceReference<T> reference &&
				   Guid.Equals(reference.Guid);
		}

		public override int GetHashCode()
		{
			return -737073652 + EqualityComparer<Guid>.Default.GetHashCode(Guid);
		}

		public static bool operator ==(ResourceReference<T> left, ResourceReference<T> right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ResourceReference<T> left, ResourceReference<T> right)
		{
			return !(left == right);
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
				.Select(x => x.GetSingle(resource => resource.GUID == guid))
				.SingleOrDefault();
		}

		public static Resource GetResourceByGuid(this Guid guid)
		{
			return ResourceManagers.Select(x => x.Key.Get(resource => resource.GUID == guid))
				.ConcatenateCollection()
				.SingleOrDefault() as Resource;
		}

		public static IResourceManager GetManagerForResource(this Guid guid)
		{
			return ResourceManagers.SingleOrDefault(x => x.Key.Get(resource => resource.GUID == guid).Any()).Key;
		}

		public static IResourceManager GetManagerForResource(this Resource resource)
		{
			return GetManagerForResource(resource.GUID);
		}

		public static bool IsAuthorized<T>(HttpRequest request, T resource, out EViewVisibility visibility) where T:Resource
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
