using Common;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace SimpleHttpServer.REST
{
	public interface IRESTResource
	{
		Guid GUID { get; }
		string ResourceURL { get; }
	}

	/// <summary>
	/// A resource is a component that can be interacted with through REST API calls
	/// It has a location on the server (given by ResourceURL) that MUST be unique
	/// It also has a UUID, which can be an alternate accessor using resources/uuid
	/// </summary>
	public abstract class Resource : IRESTResource
	{

		[NoPatch]
		[View(EPermissionLevel.USER)]
		public Guid GUID { get; private set; }
		[View(EPermissionLevel.USER)]
		public abstract string ResourceURL { get; }

		public Resource()
		{
			GUID = Guid.NewGuid();
		}

		public override bool Equals(object obj)
		{
			var resource = obj as Resource;
			return resource != null &&
				   GUID.Equals(resource.GUID) &&
				   ResourceURL == resource.ResourceURL;
		}

		public override int GetHashCode()
		{
			var hashCode = 1286416240;
			hashCode = hashCode * -1521134295 + EqualityComparer<Guid>.Default.GetHashCode(GUID);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(ResourceURL);
			return hashCode;
		}

		public virtual void OnDestroy() { }
	}

	public interface IResourceManager : IBackup
	{
		IEnumerable<Resource> Get(Func<Resource, bool> selector);
		bool IsAuthorised(HttpRequest request, Resource resource, out EPermissionLevel visibility);
		void Clear();
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
	/// </summary>
	/// <typeparam name="T">The class to be managed</typeparam>
	public abstract class ResourceManager<T> : IResourceManager<T>, IBackup where T:Resource
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
			ResourceUtility.Register(this);
		}

		public virtual bool Add(T newObject)
		{
			if(ResourceUtility.GetResourceByGuid(newObject.GUID) != null || Get(x => x.ResourceURL == newObject.ResourceURL).Any())
			{
				throw HTTPException.CONFLICT;
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

		public virtual IEnumerable<Resource> Get(Func<Resource, bool> selector)
		{
			return InternalData.Entries.Where(kvp => selector(kvp.Value)).Select(kvp => kvp.Value);
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

		public bool IsAuthorised(HttpRequest request, Resource resource, out EPermissionLevel visibility)
		{
			if(!(resource is T))
			{
				throw new Exception("Incorrect Resource Manager for " + resource);
			}
			return IsAuthorised(request, resource as T, out visibility);
		}

		protected abstract bool IsAuthorised(HttpRequest request, T resource, out EPermissionLevel visibility);
	}
}
