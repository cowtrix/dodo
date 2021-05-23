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
using System.Reflection;

namespace Resources
{
	public static class ResourceUtility
	{
		/// <summary>
		/// The key by which we get the MongoDB connection string.
		/// </summary>
		public const string CONFIGKEY_MONGODBSERVERURL = "MongoDBServerURL";
		public const string MONGODB_DATABASE_NAME = "Resources";
		/// <summary>
		/// The connection string for the MongoDB instance. Can be defined by an environment variable or through the config .json file
		/// </summary>
		private static ConfigVariable<string> m_databasePath = new ConfigVariable<string>(CONFIGKEY_MONGODBSERVERURL, "");
		/// <summary>
		/// A mapping of all resource types to their managers, which are used to interact with the underlying database.
		/// </summary>
		public static ConcurrentDictionary<Type, IResourceManager> ResourceManagers = new ConcurrentDictionary<Type, IResourceManager>();
		/// <summary>
		/// A mapping of all resource types to their factories, which are used to create new instances.
		/// </summary>
		public static ConcurrentDictionary<Type, IResourceFactory> Factories = new ConcurrentDictionary<Type, IResourceFactory>();
		/// <summary>
		/// Stores all deleted public resources, so that we can revert accidental deletions.
		/// </summary>
		public static IMongoCollection<Resource> Trash { get; private set; }
		public static PersistentStore<Guid, string> Hidden { get; private set; }
		/// <summary>
		/// The connection to the MongoDB database.
		/// </summary>
		public static MongoClient MongoDB
		{
			get
			{
				if (__mongoDBClient == null && !m_hasTriedToConnect)
				{
					m_hasTriedToConnect = true;
					var mongoDbURL = m_databasePath.Value;
					if (string.IsNullOrEmpty(mongoDbURL))
					{
						Logger.Debug($"Connected to local MongoDB server");
						__mongoDBClient = new MongoClient();
					}
					else
					{
						Logger.Debug($"Connected to MongoDB server at {mongoDbURL}");
						__mongoDBClient = new MongoClient(mongoDbURL);
					}
				}
				return __mongoDBClient;
			}
		}
		private static MongoClient __mongoDBClient;
		private static bool m_hasTriedToConnect = false;

		static ResourceUtility()
		{
			// Initialise all custom serializers
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

			// Find all types that implement IResourceManager and register them
			var types = ReflectionExtensions.GetConcreteClasses<IResourceManager>();
			foreach (var t in types)
			{
				var newManager = Activator.CreateInstance(t) as IResourceManager;
				var typeArg = newManager.GetType().BaseType.GetGenericArguments().First();
				ResourceManagers[typeArg] = newManager;
			}

			// Find all types that implement IResourceFactory and register them
			var factories = ReflectionExtensions.GetConcreteClasses<IResourceFactory>();
			foreach (var t in factories)
			{
				var newFactory = Activator.CreateInstance(t) as IResourceFactory;
				var typeArg = newFactory.GetType().BaseType.GetGenericArguments().First();
				Factories[typeArg] = newFactory;
			}

			try
			{
				// Try to initialize the connection to the trash folder for deleted resources
				var db = MongoDB.GetDatabase("Trash");
				Trash = db.GetCollection<Resource>("DeletedResources");
				Hidden = new PersistentStore<Guid, string>(MONGODB_DATABASE_NAME, nameof(Hidden));
			}
			catch (Exception e)
			{
				Logger.Exception(e, "Failed to initiialize Trash MongoDB connection");
			}
		}

		#region Resources
		/// <summary>
		/// Add the given resource to the appropriate resource manager, which will insert the resource into the database.
		/// </summary>
		/// <param name="resource">The resource to add.</param>
		public static void AddToManager(IRESTResource resource)
		{
			GetManagerForResource(resource).Add(resource);
		}

		/// <summary>
		/// Get all resources that match a given evalutation function.
		/// </summary>
		/// <typeparam name="T">The type of the resource to search for. Can handle polymorphic types, e.g. IRESTResource</typeparam>
		/// <param name="func">The function by which to evaluate the resource. Resources that return true will be returned.</param>
		/// <param name="handle">The handle of the search, if applicable. See: ResourceLock</param>
		/// <param name="ensureLatest">Should this ensure that all returned results are the latest version of that result, or just return whatever it can?</param>
		/// <returns>An enumerable of resources which satisfy the evaluator.</returns>
		public static IEnumerable<T> GetResource<T>(Func<IRESTResource, bool> func, Guid? handle = null, bool ensureLatest = false) where T : class, IRESTResource
		{
			if (func == default)
			{
				yield break;
			}
			foreach (var rm in ResourceManagers.Where(rm => rm.Value is ISearchableResourceManager)
				.OrderBy(rm => rm.Key.GetCustomAttribute<SearchPriority>()?.Priority))
			{
				var result = rm.Value.Get(x => func(x), handle, ensureLatest).OfType<T>();
				foreach (var r in result)
				{
					yield return r;
				}
			}
		}

		/// <summary>
		/// Get the first resource which fits a given evaluator.
		/// </summary>
		/// <param name="func">The function by which to evaluate the resource. Resources that return true will be returned.</param>
		/// <param name="handle">The handle of the search, if applicable. See: ResourceLock</param>
		/// <param name="ensureLatest">Should this ensure that all returned results are the latest version of that result, or just return whatever it can?</param>
		/// <returns>An enumerable of resources which satisfy the evaluator.</returns>
		public static IEnumerable<IRESTResource> GetResource(Func<IRESTResource, bool> func, Guid? handle = null, bool ensureLatest = false)
		{
			return GetResource<IRESTResource>(func, handle, ensureLatest);
		}

		/// <summary>
		/// Get a resource of type T with a given guid.
		/// </summary>
		/// <typeparam name="T">The type of the resource to search for. Can handle polymorphic types, e.g. IRESTResource</typeparam>
		/// <param name="guid">The guid of the resource.</param>
		/// <param name="handle">The handle of the search, if applicable. See: ResourceLock</param>
		/// <param name="ensureLatest">Should this ensure that all returned results are the latest version of that result, or just return whatever it can?</param>
		/// <returns>A single resource of type T with the given resource.</returns>
		public static T GetResourceByGuid<T>(this Guid guid, Guid? handle = null, bool ensureLatest = false) where T : class, IRESTResource
		{
			if (guid == default)
			{
				return default;
			}
			T result;
			foreach (var rm in ResourceManagers.OrderBy(rm => rm.Key.GetCustomAttribute<SearchPriority>()?.Priority))
			{
				result = (T)rm.Value.GetSingle(x => x.Guid == guid, handle, ensureLatest);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		/// <summary>
		/// Get a resource with a given guid.
		/// </summary>
		/// <param name="guid">The guid of the resource.</param>
		/// <param name="handle">The handle of the search, if applicable. See: ResourceLock</param>
		/// <param name="ensureLatest">Should this ensure that all returned results are the latest version of that result, or just return whatever it can?</param>
		/// <returns>A single resource with the given resource.</returns>
		public static IRESTResource GetResourceByGuid(this Guid guid, Guid? handle = null, bool ensureLatest = false)
		{
			return GetResourceByGuid<IRESTResource>(guid, handle, ensureLatest);
		}

		[Obsolete]
		public static IEnumerable<T> Search<T>(string query) where T : class, IRESTResource
		{
			if (Guid.TryParse(query, out var guid))
			{
				return new[] { GetResourceByGuid<T>(guid) };
			}
			var result = new List<T>();
			foreach (var rm in ResourceManagers.Where(rm => typeof(IPublicResource).IsAssignableFrom(rm.Key))
				.OrderBy(rm => rm.Key.GetCustomAttribute<SearchPriority>()?.Priority))
			{
				if (!typeof(T).IsAssignableFrom(rm.Key))
				{
					continue;
				}
				result.AddRange(rm.Value.Get(x => JsonConvert.SerializeObject(x).Contains(query)).Cast<T>());
			}
			return result;
		}

		/// <summary>
		/// Get a resource of type T by its slug (url identifier)
		/// </summary>
		/// <typeparam name="T">The type of the resource to search for. Can handle polymorphic types, e.g. IRESTResource</typeparam>
		/// <param name="slug">The slug of the resource.</param>
		/// <param name="handle">The handle of the search, if applicable. See: ResourceLock</param>
		/// <param name="ensureLatest">Should this ensure that all returned results are the latest version of that result, or just return whatever it can?</param>
		/// <returns>A single resource of type T with the given slug.</returns>
		public static T GetResourceBySlug<T>(this string slug, Guid? handle = null, bool ensureLatest = false) where T : class, IRESTResource
		{
			T result;
			foreach (var rm in ResourceManagers.OrderBy(rm => rm.Key.GetCustomAttribute<SearchPriority>()?.Priority))
			{
				result = (T)rm.Value.GetSingle(x => x.Slug == slug, handle, ensureLatest);
				if (result != null)
				{
					return result;
				}
			}
			return null;
		}

		/// <summary>
		/// Get a resource by its slug (url identifier)
		/// </summary>
		/// <param name="slug">The slug of the resource.</param>
		/// <param name="handle">The handle of the search, if applicable. See: ResourceLock</param>
		/// <param name="ensureLatest">Should this ensure that all returned results are the latest version of that result, or just return whatever it can?</param>
		/// <returns>A single resource with the given slug.</returns>
		public static IRESTResource GetResourceBySlug(this string slug, Guid? handle = null)
		{
			return GetResourceBySlug<IRESTResource>(slug, handle);
		}
		#endregion

		#region Managers
		/// <summary>
		/// Clear all databases that the app knows about. Use with caution - will cause data loss!
		/// </summary>
		public static void ClearAllManagers()
		{
			Logger.Warning($"Purging MongoDB database");
			foreach (var rm in ResourceManagers)
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
			if (ResourceManagers.TryGetValue(type, out var factory))
			{
				return factory;
			}
			if (type.BaseType != null)
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
			if (Factories.TryGetValue(type, out var factory))
			{
				return factory;
			}
			if (type.BaseType != null)
			{
				return GetFactory(type.BaseType);
			}
			throw new Exception($"No factory found for type {type}");
		}
		#endregion
	}

}
