using Common;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

// Q: If we did move to a database, would it be possible to keep the nice LINQ stuff here? - Sean

namespace SimpleHttpServer.REST
{
	public interface IResourceManager : IBackup
	{
		IRESTResource GetSingle(Func<IRESTResource, bool> selector);
		IRESTResource GetFirst(Func<IRESTResource, bool> selector);
		IEnumerable<IRESTResource> Get(Func<IRESTResource, bool> selector);
		bool IsAuthorised(HttpRequest request, IRESTResource resource, out EPermissionLevel visibility);
		void Clear();
		bool Add(IRESTResource newObject);
		void Set(Guid gUID, IRESTResource resource);
	}

	public interface IResourceManager<T> : IResourceManager
	{
		bool Add(T newObject);
		bool Delete(T objToDelete);
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
	public abstract class ResourceManager<T> : IResourceManager<T>, IBackup where T: class, IRESTResource
	{
		public class Data
		{
			public ConcurrentDictionary<Guid, T> Entries = new ConcurrentDictionary<Guid, T>();
			public ConcurrentDictionary<Guid, T> Archive = new ConcurrentDictionary<Guid, T>();
		}
		protected Data InternalData;

		public abstract string BackupPath { get; }

		public ResourceManager()
		{
			InternalData = InternalData ?? new Data();	// This is to support custom child class implementations of this::Data
		}

		public virtual bool Add(T newObject)
		{
			if(ResourceUtility.GetResourceByGuid(newObject.GUID) != null || Get(x => x.ResourceURL == newObject.ResourceURL).Any())
			{
				throw HttpException.CONFLICT;
			}
			return InternalData.Entries.TryAdd(newObject.GUID, newObject);
		}

		public virtual bool Delete(T objToDelete)
		{
			objToDelete.OnDestroy();
			return InternalData.Entries.TryRemove(objToDelete.GUID, out var deletedEntry)
				&& InternalData.Archive.TryAdd(deletedEntry.GUID, deletedEntry);
		}

		public virtual T GetSingle(Func<T, bool> selector)
		{
			return InternalData.Entries.SingleOrDefault(kvp => selector(kvp.Value)).Value;
		}

		public virtual T GetFirst(Func<T, bool> selector)
		{
			return InternalData.Entries.FirstOrDefault(kvp => selector(kvp.Value)).Value;
		}

		public virtual IEnumerable<T> Get(Func<T, bool> selector)
		{
			return InternalData.Entries.Where(kvp => selector(kvp.Value)).Select(kvp => kvp.Value);
		}

		public virtual IEnumerable<IRESTResource> Get(Func<IRESTResource, bool> selector)
		{
			return InternalData.Entries.Where(kvp => selector(kvp.Value)).Select(kvp => (IRESTResource)kvp.Value);
		}

		public virtual void Clear()
		{
			InternalData = new Data();
		}

		public string Serialize()
		{
			return JsonConvert.SerializeObject(InternalData, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
		}

		public void Deserialize(string json)
		{
			InternalData = JsonConvert.DeserializeObject(json) as Data;
		}

		public bool IsAuthorised(HttpRequest request, IRESTResource resource, out EPermissionLevel permission)
		{
			if(resource == null)
			{
				permission = EPermissionLevel.PUBLIC;
				return true;
			}
			if(!(resource is T))
			{
				throw new Exception("Incorrect Resource Manager for " + resource);
			}
			return IsAuthorised(request, resource as T, out permission);
		}

		protected abstract bool IsAuthorised(HttpRequest request, T resource, out EPermissionLevel visibility);

		public IRESTResource GetSingle(Func<IRESTResource, bool> selector)
		{
			return InternalData.Entries.SingleOrDefault(kvp => selector(kvp.Value)).Value;
		}

		public IRESTResource GetFirst(Func<IRESTResource, bool> selector)
		{
			return InternalData.Entries.FirstOrDefault(kvp => selector(kvp.Value)).Value;
		}

		public bool Add(IRESTResource newObject)
		{
			return Add((T)newObject);
		}

		public void Set(Guid guid, IRESTResource resource)
		{
			InternalData.Entries[guid] = (T)resource;
		}
	}
}
