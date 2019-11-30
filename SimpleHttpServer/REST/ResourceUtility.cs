using Common;
using Common.Extensions;
using SimpleHttpServer.Models;
using System;
using System.Collections.Concurrent;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleHttpServer.REST
{

	public static class ResourceUtility
	{
		private static ConcurrentDictionary<Type, IResourceManager> ResourceManagers = new ConcurrentDictionary<Type, IResourceManager>();

		static ResourceUtility()
		{
			var types = ReflectionExtensions.GetChildClasses<IResourceManager>();
			foreach(var t in types)
			{
				var newManager = Activator.CreateInstance(t) as IResourceManager;
				Register(newManager);
			}
		}
  
		private static void Register(IResourceManager manager)
		{
			var typeArg = manager.GetType().BaseType.GetGenericArguments().First();
			ResourceManagers[typeArg] = manager;
		}

		public static void Register(IRESTResource resource)
		{
			GetManagerForResource(resource).Add(resource);
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

		public static IResourceManager GetManagerForResource(this IRESTResource resource)
		{
			return ResourceManagers.SingleOrDefault(x => resource.GetType().IsAssignableFrom(x.Key)).Value;
		}

		public static IResourceManager GetManagerForResource(this Guid guid)
		{
			return ResourceManagers.SingleOrDefault(x => x.Value.Get(resource => resource.GUID == guid).Any()).Value;
		}

		public static IRESTResource GetResourceByURL(string url)
		{
			return ResourceManagers.Select(rm => rm.Value.GetSingle(x => x.ResourceURL == url))
				.SingleOrDefault(x => x != null);
		}

		public static IResourceManager GetManagerForResource(this Resource resource)
		{
			return GetManagerForResource(resource.GUID);
		}

		public static IResourceManager<T> GetManager<T>() where T:IRESTResource
		{
			return ResourceManagers[typeof(T)] as IResourceManager<T>;
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

		public static void Clear()
		{
			foreach(var rm in ResourceManagers)
			{
				rm.Value.Clear();
			}
		}
	}
}
