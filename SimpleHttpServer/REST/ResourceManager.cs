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
		Guid UUID { get; }
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
		[View]
		public Guid UUID { get; private set; }
		[View]
		public abstract string ResourceURL { get; }

		public Resource()
		{
			UUID = Guid.NewGuid();
		}
	}

	public interface IResourceManager
	{
		IEnumerable<Resource> Get(Func<Resource, bool> selector);
		bool IsAuthorised(HttpRequest request, Resource resource);
	}

	public abstract class ResourceManager<T> : IResourceManager, IBackup where T:Resource
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
			InternalData = InternalData ?? new Data();
			ResourceUtility.Register(this);
		}

		public virtual bool Add(T newObject)
		{
			if(ResourceUtility.GetResourceByGuid(newObject.UUID) != null || Get(x => x.ResourceURL == newObject.ResourceURL).Any())
			{
				throw HTTPException.CONFLICT;
			}
			return InternalData.Entries.TryAdd(newObject.UUID, newObject);
		}

		public virtual bool Delete(T objToDelete)
		{
			return InternalData.Entries.TryRemove(objToDelete.UUID, out var deletedEntry)
				&& InternalData.Archive.TryAdd(deletedEntry.UUID, deletedEntry);
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

		public bool IsAuthorised(HttpRequest request, Resource resource)
		{
			if(!(resource is T))
			{
				throw new Exception("Incorrect Resource Manager for " + resource);
			}
			return IsAuthorised(request, resource as T);
		}

		protected abstract bool IsAuthorised(HttpRequest request, T resource);
	}
}
