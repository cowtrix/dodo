using Common;
using Common.Config;
using Common.Extensions;
using MongoDB.Driver;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Threading;

namespace SimpleHttpServer.REST
{
	public interface IResourceManager
	{
		bool IsAuthorised(HttpRequest request, IRESTResource resource, out EPermissionLevel visibility);
		void Clear();
		void Add(IRESTResource newObject);
		void Update(IRESTResource objToUpdate, ResourceLock locker);
		IRESTResource GetSingle(Func<IRESTResource, bool> selector);
		IRESTResource GetFirst(Func<IRESTResource, bool> selector);
		IEnumerable<IRESTResource> Get(Func<IRESTResource, bool> selector);
	}

	public interface IResourceManager<T> : IResourceManager
	{
		void Add(T newObject);
		void Delete(T objToDelete);
		void Update(T objToUpdate, ResourceLock locker);
		T GetSingle(Func<T, bool> selector);
		T GetFirst(Func<T, bool> selector);
		IEnumerable<T> Get(Func<T, bool> selector);
	}

	/// <summary>
	/// A ResourceManager keeps track of, deletes, creates, updates and generally manages a type of resource.
	/// </summary>
	/// <typeparam name="T">The class to be managed</typeparam>
	public abstract class ResourceManager<T> : IResourceManager<T> where T: class, IRESTResource
	{
		/// <summary>
		/// How long a request will wait to get access to a resource before giving up, in miliseconds.
		/// See: WaitForUnlocked
		/// </summary>
		private ConfigVariable<int> m_resourceLockTimeoutMs = new ConfigVariable<int>("ResourceLockTimeout", 10 * 1000);
		private IMongoCollection<T> m_db;

		public ResourceManager()
		{
			// Connect to the database
			var database = ResourceUtility.MongoDB.GetDatabase(MongoDBDatabaseName);
			// Get the collection (which is the name of this type by default)
			m_db = database.GetCollection<T>(MongoDBCollectionName);
			// Create an index of the GUID
			// TODO investigate ResourceURL commented out bit - ResourceURL not currently serialized so can't be used
			// TODO investigate if we need to do this every time or if it will create a new index every time
			var indexOptions = new CreateIndexOptions();
			var indexKeys = Builders<T>.IndexKeys//.Ascending(rsc => rsc.ResourceURL)
				.Ascending(rsc => rsc.GUID);
			var indexModel = new CreateIndexModel<T>(indexKeys, indexOptions);
			m_db.Indexes.CreateOne(indexModel);
		}

		protected abstract string MongoDBDatabaseName { get; }
		protected virtual string MongoDBCollectionName { get { return typeof(T).Name; } }

		/// <summary>
		/// Add a new Resource to the database.
		/// </summary>
		/// <param name="newObject"></param>
		public virtual void Add(T newObject)
		{
			if(ResourceUtility.GetResourceByGuid(newObject.GUID) != null || Get(x => x.ResourceURL == newObject.ResourceURL).Any())
			{
				throw HttpException.CONFLICT;
			}
			m_db.InsertOne(newObject);
		}

		/// <summary>
		/// Delete a Resource from the database.
		/// </summary>
		/// <param name="objToDelete"></param>
		public virtual void Delete(T objToDelete)
		{
			objToDelete.OnDestroy();
			m_db.DeleteOne(x => x.GUID == objToDelete.GUID);
		}

		/// <summary>
		/// Update a Resource in the database.
		/// </summary>
		/// <param name="objToUpdate">The object to update.</param>
		/// <param name="locker">The ResourceLock of the object (to guarantee someone else isn't editing it)</param>
		public virtual void Update(T objToUpdate, ResourceLock locker)
		{
			if(locker.Guid != objToUpdate.GUID)
			{
				// This should never, ever happen in normal execution of the program
				throw new Exception("Locker GUID mismatch");
			}
			m_db.ReplaceOne(x => x.GUID == objToUpdate.GUID, objToUpdate);
		}

		/// <summary>
		/// Get a single resource with the given selector.
		/// This will throw an exception if more than one resource is found.
		/// </summary>
		/// <param name="selector">A lambda function to search with.</param>
		/// <returns>A resource of type T that satisfies the selector</returns>
		public virtual T GetSingle(Func<T, bool> selector)
		{
			return WaitForUnlocked(m_db.AsQueryable().SingleOrDefault(selector));
		}

		/// <summary>
		/// Get the first resource with the given selector.
		/// </summary>
		/// <param name="selector">A lambda function to search with.</param>
		/// <returns>A resource of type T that satisfies the selector</returns>
		public virtual T GetFirst(Func<T, bool> selector)
		{
			return WaitForUnlocked(m_db.AsQueryable().FirstOrDefault(selector));
		}

		/// <summary>
		/// Get all resources that match a given selector.
		/// </summary>
		/// <param name="selector">A lambda function to search with.</param>
		/// <returns>An enumerable of resources that satisfy the selector</returns>
		public virtual IEnumerable<T> Get(Func<T, bool> selector)
		{
			return WaitForAllUnlocked(m_db.AsQueryable().Where(selector));
		}

		/// <summary>
		/// Clear all data from the database. USE WITH CAUTION - this will permanently delete data from the database.
		/// </summary>
		public virtual void Clear()
		{
			m_db.Database.DropCollection(typeof(T).Name);
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
			if(resource != null && !(resource is T))
			{
				throw new Exception("Incorrect Resource Manager for " + resource);
			}
			return IsAuthorised(request, resource as T, out permission);
		}

		protected abstract bool IsAuthorised(HttpRequest request, T resource, out EPermissionLevel visibility);

		void IResourceManager.Add(IRESTResource newObject)
		{
			Add((T)newObject);
		}

		void IResourceManager.Update(IRESTResource newObject, ResourceLock locker)
		{
			Update((T)newObject, locker);
		}

		IRESTResource IResourceManager.GetSingle(Func<IRESTResource, bool> selector)
		{
			return m_db.AsQueryable().SingleOrDefault(selector);
		}

		IRESTResource IResourceManager.GetFirst(Func<IRESTResource, bool> selector)
		{
			return m_db.AsQueryable().FirstOrDefault(selector);
		}

		IEnumerable<IRESTResource> IResourceManager.Get(Func<IRESTResource, bool> selector)
		{
			return m_db.AsQueryable().Where(selector);
		}

		IEnumerable<T> WaitForAllUnlocked(IEnumerable<T> enumerable)
		{
			foreach (var rsc in enumerable)
			{
				WaitForUnlocked(rsc);
			}
			return enumerable;
		}

		T WaitForUnlocked(T resource)
		{
			var sw = new Stopwatch();
			sw.Start();
			while (ResourceLock.IsLocked(resource))
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
