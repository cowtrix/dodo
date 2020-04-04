using Common;
using Common.Config;
using Common.Extensions;
using Microsoft.AspNetCore.Http;
using MongoDB.Bson.Serialization;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace Resources
{
	public interface IResourceManager
	{
		void Clear();
		void Add(IRESTResource newObject);
		void Update(IRESTResource objToUpdate, ResourceLock locker);
		IRESTResource GetSingle(Func<IRESTResource, bool> selector, Guid? handle = null);
		IRESTResource GetFirst(Func<IRESTResource, bool> selector, Guid? handle = null);
		IEnumerable<IRESTResource> Get(Func<IRESTResource, bool> selector, Guid? handle = null);
	}

	public interface IResourceManager<T> : IResourceManager
	{
		void Add(T newObject);
		void Delete(T objToDelete);
		void Update(T objToUpdate, ResourceLock locker);
		T GetSingle(Func<T, bool> selector, Guid? handle = null);
		T GetFirst(Func<T, bool> selector, Guid? handle = null);
		IEnumerable<T> Get(Func<T, bool> selector, Guid? handle = null);
	}

	/// <summary>
	/// A ResourceManager keeps track of, deletes, creates, updates and generally manages a type of resource.
	/// </summary>
	/// <typeparam name="T">The class to be managed</typeparam>
	public abstract class ResourceManager<T> : IResourceManager<T> where T : class, IRESTResource
	{
		/// <summary>
		/// How long a request will wait to get access to a resource before giving up, in miliseconds.
		/// See: WaitForUnlocked
		/// </summary>
		private ConfigVariable<int> m_resourceLockTimeoutMs = new ConfigVariable<int>("ResourceLockTimeout", 10 * 1000);
		public IMongoCollection<T> MongoDatabase { get; private set; }

		public ResourceManager()
		{
			// Connect to the database
			var database = ResourceUtility.MongoDB.GetDatabase(MongoDBDatabaseName);
			// Get the collection (which is the name of this type by default)
			MongoDatabase = database.GetCollection<T>(MongoDBCollectionName);

			foreach (var type in ReflectionExtensions.GetConcreteClasses<T>())
			{
				RegisterMapIfNeeded(type);
			}

			// Create an index of the GUID
			var indexOptions = new CreateIndexOptions();
			var indexKeys = Builders<T>.IndexKeys
				.Ascending(rsc => rsc.Guid);
			var indexModel = new CreateIndexModel<T>(indexKeys, indexOptions);
			MongoDatabase.Indexes.CreateOne(indexModel);
		}

		// Check to see if map is registered before registering class map
		// This is for the sake of the polymorphic types that we are using so Mongo knows how to deserialize
		private void RegisterMapIfNeeded(Type t)
		{
			if (!BsonClassMap.IsClassMapRegistered(t))
				BsonClassMap.RegisterClassMap(new BsonClassMap(t));
		}

		protected abstract string MongoDBDatabaseName { get; }
		protected virtual string MongoDBCollectionName { get { return typeof(T).Name; } }

		/// <summary>
		/// Add a new Resource to the database.
		/// </summary>
		/// <param name="newObject"></param>
		public virtual void Add(T newObject)
		{
			if (ResourceUtility.GetResourceByGuid(newObject.Guid) != null)
			{
				throw new Exception("Conflicting GUID");
			}
			MongoDatabase.InsertOne(newObject);
		}

		/// <summary>
		/// Delete a Resource from the database.
		/// </summary>
		/// <param name="objToDelete"></param>
		public virtual void Delete(T objToDelete)
		{
			objToDelete.OnDestroy();
			MongoDatabase.DeleteOne(x => x.Guid == objToDelete.Guid);
		}

		/// <summary>
		/// Update a Resource in the database.
		/// </summary>
		/// <param name="objToUpdate">The object to update.</param>
		/// <param name="locker">The ResourceLock of the object (to guarantee someone else isn't editing it)</param>
		public virtual void Update(T objToUpdate, ResourceLock locker)
		{
			if (locker.Guid != objToUpdate.Guid)
			{
				// This should never, ever happen in normal execution of the program
				throw new Exception("Locker GUID mismatch");
			}
			MongoDatabase.ReplaceOne(x => x.Guid == objToUpdate.Guid, objToUpdate);
		}

		/// <summary>
		/// Get a single resource with the given selector.
		/// This will throw an exception if more than one resource is found.
		/// </summary>
		/// <param name="selector">A lambda function to search with.</param>
		/// <returns>A resource of type T that satisfies the selector</returns>
		public virtual T GetSingle(Func<T, bool> selector, Guid? handle = null)
		{
			return WaitForUnlocked(MongoDatabase.AsQueryable().SingleOrDefault(selector), handle) as T;
		}

		/// <summary>
		/// Get the first resource with the given selector.
		/// </summary>
		/// <param name="selector">A lambda function to search with.</param>
		/// <returns>A resource of type T that satisfies the selector</returns>
		public virtual T GetFirst(Func<T, bool> selector, Guid? handle = null)
		{
			return WaitForUnlocked(MongoDatabase.AsQueryable().FirstOrDefault(selector), handle) as T;
		}

		/// <summary>
		/// Get all resources that match a given selector.
		/// </summary>
		/// <param name="selector">A lambda function to search with.</param>
		/// <returns>An enumerable of resources that satisfy the selector</returns>
		public virtual IEnumerable<T> Get(Func<T, bool> selector, Guid? handle = null)
		{
			return WaitForAllUnlocked(MongoDatabase.AsQueryable().Where(selector), handle).Cast<T>();
		}

		/// <summary>
		/// Clear all data from the database. USE WITH CAUTION - this will permanently delete data from the database.
		/// </summary>
		public virtual void Clear()
		{
			MongoDatabase.Database.DropCollection(typeof(T).Name);
		}

		/// <summary>
		/// Determine if a request is authorised, and with which permission level.
		/// </summary>
		/// <param name="request">The request to be evaluated</param>
		/// <param name="resource">The resource that is being interacted with</param>
		/// <param name="permission">The permission level that this request has</param>
		/// <returns>Whether this request is authorised (a value of false should result in a HttpException.FORBIDDEN exception being thrown)</returns>
		public bool IsAuthorised(HttpRequest request, IRESTResource resource, out EPermissionLevel permission)
		{
			if (resource != null && !(resource is T))
			{
				throw new Exception("Incorrect Resource Manager for " + resource);
			}
			return IsAuthorised(request, resource as T, out permission);
		}

		void IResourceManager.Add(IRESTResource newObject)
		{
			Add((T)newObject);
		}

		void IResourceManager.Update(IRESTResource newObject, ResourceLock locker)
		{
			Update((T)newObject, locker);
		}

		IRESTResource IResourceManager.GetSingle(Func<IRESTResource, bool> selector, Guid? handle = null)
		{
			return WaitForUnlocked(MongoDatabase.AsQueryable().SingleOrDefault(selector), handle);
		}

		IRESTResource IResourceManager.GetFirst(Func<IRESTResource, bool> selector, Guid? handle = null)
		{
			return WaitForUnlocked(MongoDatabase.AsQueryable().FirstOrDefault(selector), handle);
		}

		IEnumerable<IRESTResource> IResourceManager.Get(Func<IRESTResource, bool> selector, Guid? handle = null)
		{
			return WaitForAllUnlocked(MongoDatabase.AsQueryable().Where(selector), handle);
		}

		IEnumerable<IRESTResource> WaitForAllUnlocked(IEnumerable<IRESTResource> enumerable, Guid? handle = null)
		{
			foreach (var rsc in enumerable)
			{
				WaitForUnlocked(rsc, handle);
			}
			return enumerable;
		}

		IRESTResource WaitForUnlocked(IRESTResource resource, Guid? handle)
		{
			var sw = new Stopwatch();
			sw.Start();
			while (ResourceLock.IsLocked(resource, handle))
			{
				if (sw.Elapsed.TotalMilliseconds > m_resourceLockTimeoutMs.Value)
				{
					throw new Exception("Resource Locked");
				}
				Thread.Sleep(20);
			}
			return resource;
		}
	}
}
