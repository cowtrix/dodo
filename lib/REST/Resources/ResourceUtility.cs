using Common.Config;
using Common.Extensions;
using Microsoft.AspNetCore.Http;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace REST
{

	public static class ResourceUtility
	{
		private static ConfigVariable<string> m_databasePath = new ConfigVariable<string>("MongoDBServerURL", "");
		private static ConcurrentDictionary<Type, IResourceManager> ResourceManagers = new ConcurrentDictionary<Type, IResourceManager>();
		private static ConcurrentDictionary<Type, IResourceFactory> Factories = new ConcurrentDictionary<Type, IResourceFactory>();
		public static MongoClient MongoDB { get; private set; }

		static ResourceUtility()
		{
			var mongoDbURL = m_databasePath.Value;
			if(string.IsNullOrEmpty(mongoDbURL))
			{
				MongoDB = new MongoClient();
			}
			else
			{
				MongoDB = new MongoClient(mongoDbURL);
			}

			var types = ReflectionExtensions.GetChildClasses<IResourceManager>();
			foreach(var t in types)
			{
				var newManager = Activator.CreateInstance(t) as IResourceManager;
				var typeArg = newManager.GetType().BaseType.GetGenericArguments().First();
				ResourceManagers[typeArg] = newManager;
			}

			types = ReflectionExtensions.GetChildClasses<IResourceFactory>();
			foreach (var t in types)
			{
				var newFactory = Activator.CreateInstance(t) as IResourceFactory;
				var typeArg = newFactory.GetType().BaseType.GetGenericArguments().First();
				Factories[typeArg] = newFactory;
			}
		}

		#region Resources
		public static void Register(IRESTResource resource)
		{
			GetManagerForResource(resource).Add(resource);
		}

		public static T GetResourceByGuid<T>(this Guid guid) where T : class, IRESTResource
		{
			T result;
			foreach(var rm in ResourceManagers)
			{
				result = (T)rm.Value.GetSingle(x => x.GUID == guid);
				if(result != null)
				{
					return result;
				}
			}
			return null;
		}

		public static IRESTResource GetResourceByGuid(this Guid guid)
		{
			return GetResourceByGuid<IRESTResource>(guid);
		}

		public static T GetResourceByURL<T>(string url) where T : class, IRESTResource
		{
			T result;
			foreach (var rm in ResourceManagers)
			{
				result = (T)rm.Value.GetSingle(x => x.ResourceURL == url);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		public static IRESTResource GetResourceByURL(string url)
		{
			return GetResourceByURL<IRESTResource>(url);
		}

		public static IEnumerable<T> Search<T>(string query) where T : class, IRESTResource
		{
			if (Guid.TryParse(query, out var guid))
			{
				return new[] { GetResourceByGuid<T>(guid) };
			}
			var result = new List<T>();
			foreach (var rm in ResourceManagers)
			{
				if (!typeof(T).IsAssignableFrom(rm.Key))
				{
					continue;
				}
				result.AddRange(rm.Value.Get(x => JsonConvert.SerializeObject(x).Contains(query)).Cast<T>());
			}
			return result;
		}
		#endregion
		#region Managers
		public static void ClearAllManagers()
		{
			foreach(var rm in ResourceManagers)
			{
				rm.Value.Clear();
			}
		}

		public static IResourceManager GetManagerForResource(this IRESTResource resource)
		{
			return ResourceManagers.SingleOrDefault(x => x.Key.IsAssignableFrom(resource.GetType())).Value;
		}

		public static IResourceManager GetManagerForResource(this Guid guid)
		{
			return ResourceManagers.SingleOrDefault(x => x.Value.Get(resource => resource.GUID == guid).Any()).Value;
		}

		public static IResourceManager GetManagerForResource(this Resource resource)
		{
			return GetManagerForResource(resource.GUID);
		}

		public static IResourceManager<T> GetManager<T>() where T : IRESTResource
		{
			return ResourceManagers[typeof(T)] as IResourceManager<T>;
		}
		#endregion
		#region Factories
		public static IResourceFactory<T> GetFactory<T>() where T : IRESTResource
		{
			return Factories[typeof(T)] as IResourceFactory<T>;
		}
		#endregion
	}

}
