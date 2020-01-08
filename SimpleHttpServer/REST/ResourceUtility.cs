using Common;
using Common.Config;
using Common.Extensions;
using MongoDB.Driver;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace SimpleHttpServer.REST
{

	public static class ResourceUtility
	{
		private static ConfigVariable<string> m_databasePath = new ConfigVariable<string>("MongoDBServerURL", "");
		private static ConcurrentDictionary<Type, IResourceManager> ResourceManagers = new ConcurrentDictionary<Type, IResourceManager>();
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
			var rm = GetManagerForResource(guid);
			return rm?.GetSingle(resource => resource.GUID == guid);
		}

		public static IResourceManager GetManagerForResource(this IRESTResource resource)
		{
			return ResourceManagers.SingleOrDefault(x => x.Key.IsAssignableFrom(resource.GetType())).Value;
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

		const string SEARCH_CMD_REGEX = @"^resource\ssearch\s(.*)";
		[Command(SEARCH_CMD_REGEX, "resource search", "Search for a resource")]
		public static void SearchCommand(string cmd)
		{
			var rgx = Regex.Match(cmd, SEARCH_CMD_REGEX);
			var query = rgx.Groups[1].Value;
			Logger.Debug(JsonConvert.SerializeObject(Search<IRESTResource>(cmd), JsonExtensions.DatabaseSettings));
		}

		public static IEnumerable<T> Search<T>(string query) where T : class, IRESTResource
		{
			if (Guid.TryParse(query, out var guid))
			{
				return new[] { GetResourceByGuid<T>(guid) };
			}
			return null;
		}
	}
}
