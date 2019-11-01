﻿using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Linq;

namespace Dodo
{
	public class ResourceManager<T> where T:Resource
	{
		public class Data
		{
			public ConcurrentDictionary<Guid, T> Entries = new ConcurrentDictionary<Guid, T>();
		}
		protected Data InternalData;

		public ResourceManager()
		{
			InternalData = InternalData ?? new Data();
		}

		public virtual T GetByGUID(Guid guid)
		{
			InternalData.Entries.TryGetValue(guid, out var result);
			return result;
		}

		public virtual bool Delete(T objToDelete)
		{
			return InternalData.Entries.TryRemove(objToDelete.UUID, out _);
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
	}
}
