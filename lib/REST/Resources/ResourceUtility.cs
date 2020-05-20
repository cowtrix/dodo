using Common;
using Common.Config;
using Common.Extensions;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using Resources.Serializers;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Resources
{
	public static class ResourceUtility
	{
		public const string CONFIGKEY_MONGODBSERVERURL = "MongoDBServerURL";
		private static ConfigVariable<string> m_databasePath = new ConfigVariable<string>(CONFIGKEY_MONGODBSERVERURL, "");
		public static ConcurrentDictionary<Type, IResourceManager> ResourceManagers = new ConcurrentDictionary<Type, IResourceManager>();
		public static ConcurrentDictionary<Type, IResourceFactory> Factories = new ConcurrentDictionary<Type, IResourceFactory>();
		public static MongoClient MongoDB { get; private set; }

		static ResourceUtility()
		{
			var customSerializers = ReflectionExtensions.GetConcreteClasses<ICustomBsonSerializer>();
			foreach (var customSer in customSerializers)
			{
				var newSerializer = Activator.CreateInstance(customSer) as IBsonSerializer;
				try
				{
					BsonSerializer.RegisterSerializer(newSerializer.ValueType, newSerializer);
				}
				catch { }   // TODO: catch these failures better, they happen when testing
			}

			var mongoDbURL = m_databasePath.Value;
			if(string.IsNullOrEmpty(mongoDbURL))
			{
				Logger.Debug($"Connected to local MongoDB server");
				MongoDB = new MongoClient();
			}
			else
			{
				Logger.Debug($"Connected to MongoDB server at {mongoDbURL}");
				MongoDB = new MongoClient(mongoDbURL);
			}

			var types = ReflectionExtensions.GetConcreteClasses<IResourceManager>();
			foreach(var t in types)
			{
				var newManager = Activator.CreateInstance(t) as IResourceManager;
				var typeArg = newManager.GetType().BaseType.GetGenericArguments().First();
				Register(typeArg, newManager);
			}

			types = ReflectionExtensions.GetConcreteClasses<IResourceFactory>();
			foreach (var t in types)
			{
				var newFactory = Activator.CreateInstance(t) as IResourceFactory;
				var typeArg = newFactory.GetType().BaseType.GetGenericArguments().First();
				Register(typeArg, newFactory);
			}
		}

		public static void Register(Type type, IResourceManager resourceManager)
		{
			ResourceManagers[type] = resourceManager;
		}

		public static void Register(Type type, IResourceFactory factory)
		{
			Factories[type] = factory;
		}

		#region Resources
		public static void Register(IRESTResource resource)
		{
			GetManagerForResource(resource).Add(resource);
		}

		public static T GetResourceByGuid<T>(this Guid guid, Guid? handle = null) where T : class, IRESTResource
		{
			T result;
			foreach(var rm in ResourceManagers)
			{
				result = (T)rm.Value.GetSingle(x => x.Guid == guid, handle);
				if(result != null)
				{
					return result;
				}
			}
			return null;
		}

		public static IRESTResource GetResourceByGuid(this Guid guid, Guid? handle = null)
		{
			return GetResourceByGuid<IRESTResource>(guid, handle);
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

		public static T GetResourceBySlug<T>(this string slug, Guid? handle = null) where T : class, IRESTResource
		{
			T result;
			foreach (var rm in ResourceManagers)
			{
				result = (T)rm.Value.GetSingle(x => x.Slug == slug, handle);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		public static IRESTResource GetResourceBySlug(this string slug, Guid? handle = null)
		{
			return GetResourceBySlug<IRESTResource>(slug, handle);
		}

		#endregion
		#region Managers
		public static void ClearAllManagers()
		{
			Logger.Warning($"Purging MongoDB database");
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
			return ResourceManagers.SingleOrDefault(x => x.Value.Get(resource => resource.Guid == guid).Any()).Value;
		}

		public static IResourceManager GetManagerForResource(this Resource resource)
		{
			return GetManager(resource.GetType());
		}

		public static IResourceManager<T> GetManager<T>()
		{
			return GetManager(typeof(T)) as IResourceManager<T>;
		}

		public static IResourceManager GetManager(Type type)
		{
			if(ResourceManagers.TryGetValue(type, out var factory))
			{
				return factory;
			}
			if(type.BaseType != null)
			{
				return GetManager(type.BaseType);
			}
			throw new Exception($"No factory found for type {type}");
		}
		#endregion
		#region Factories
		public static IResourceFactory<T> GetFactory<T>() where T : IRESTResource
		{
			return GetFactory(typeof(T)) as IResourceFactory<T>;
		}

		
		public static IResourceFactory GetFactory(Type type)
		{
			if(Factories.TryGetValue(type, out var factory))
			{
				return factory;
			}
			if(type.BaseType != null)
			{
				return GetFactory(type.BaseType);
			}
			throw new Exception($"No factory found for type {type}");
		}
		#endregion
	}

}
