using Common;
using Common.Extensions;
using MongoDB.Driver;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

// Q: If we did move to a database, would it be possible to keep the nice LINQ stuff here? - Sean

namespace SimpleHttpServer.REST
{
	public interface IResourceManager
	{
		bool IsAuthorised(HttpRequest request, IRESTResource resource, out EPermissionLevel visibility);
		void Clear();
		void Add(IRESTResource newObject);
		void Update(IRESTResource objToUpdate);
		IRESTResource GetSingle(Func<IRESTResource, bool> selector);
		IRESTResource GetFirst(Func<IRESTResource, bool> selector);
		IEnumerable<IRESTResource> Get(Func<IRESTResource, bool> selector);
	}

	public interface IResourceManager<T> : IResourceManager
	{
		void Add(T newObject);
		void Delete(T objToDelete);
		void Update(T objToUpdate);
		T GetSingle(Func<T, bool> selector);
		T GetFirst(Func<T, bool> selector);
		IEnumerable<T> Get(Func<T, bool> selector);
	}

	/// <summary>
	/// A ResourceManager keeps track of, deletes, creates and generally manages a class of object.
	/// If we were going to transition to a database, this would be the thing that manages that.
	/// But currently we just hold everything in JSON and write it out to a file.
	/// </summary>
	/// <typeparam name="T">The class to be managed</typeparam>
	public abstract class ResourceManager<T> : IResourceManager<T> where T: class, IRESTResource
	{
		private IMongoCollection<T> m_db;
		public ResourceManager()
		{
			var database = ResourceUtility.MongoDB.GetDatabase("Dodo");
			m_db = database.GetCollection<T>(typeof(T).Name);
		}

		public virtual void Add(T newObject)
		{
			if(ResourceUtility.GetResourceByGuid(newObject.GUID) != null || Get(x => x.ResourceURL == newObject.ResourceURL).Any())
			{
				throw HttpException.CONFLICT;
			}
			m_db.InsertOne(newObject);
		}

		public virtual void Delete(T objToDelete)
		{
			objToDelete.OnDestroy();
			m_db.DeleteOne(x => x.GUID == objToDelete.GUID);
		}

		public virtual void Update(T objToUpdate)
		{
			m_db.ReplaceOne(x => x.GUID == objToUpdate.GUID, objToUpdate);
		}

		public virtual T GetSingle(Func<T, bool> selector)
		{
			return m_db.AsQueryable().SingleOrDefault(selector);
		}

		public virtual T GetFirst(Func<T, bool> selector)
		{
			return m_db.AsQueryable().FirstOrDefault(selector);
		}

		public virtual IEnumerable<T> Get(Func<T, bool> selector)
		{
			return m_db.AsQueryable().Where(selector);
		}

		public virtual void Clear()
		{
			m_db.Database.DropCollection(typeof(T).Name);
		}

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

		void IResourceManager.Update(IRESTResource newObject)
		{
			Update((T)newObject);
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
	}
}
