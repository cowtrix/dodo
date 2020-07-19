using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace Resources
{
	public class PersistentStore<TKey, TValue>
	{
		public class Entry
		{
			[BsonId]
			public TKey Key;
			[BsonElement]
			public TValue Value;

			public Entry(TKey key, TValue value)
			{
				Key = key;
				Value = value;
			}
		}

		public IMongoCollection<Entry> Collection;

		public PersistentStore(string dbName, string collectionName)
		{
			Collection = ResourceUtility.MongoDB.GetDatabase(dbName).GetCollection<Entry>(collectionName);
			var indexOptions = new CreateIndexOptions();
			var indexKeys = Builders<Entry>.IndexKeys
				.Ascending(rsc => rsc.Key);
			var indexModel = new CreateIndexModel<Entry>(indexKeys, indexOptions);
			Collection.Indexes.CreateOne(indexModel);
		}

		public TValue this[TKey key]    // Indexer declaration  
		{
			get
			{
				return Collection.Find(x => x.Key.Equals(key)).First().Value;
			}
			set
			{
				Collection.ReplaceOne(e => e.Key.Equals(key), new Entry(key, value), new ReplaceOptions() { IsUpsert = true });
			}
		}

		public void Clear()
		{
			Collection.Database.DropCollection(Collection.CollectionNamespace.CollectionName);
		}

		public IMongoQueryable<Entry> GetQueryable()
		{
			return Collection.AsQueryable();
		}

		public bool Remove(TKey key)
		{
			return Collection.DeleteOne(x => x.Key.Equals(key)).IsAcknowledged;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			var match = Collection.Find(x => x.Key.Equals(key));
			if(!match.Any())
			{
				value = default;
				return false;
			}
			value = match.First().Value;
			return true;
		}

		public bool ContainsKey(TKey key)
		{
			var count = Collection.CountDocuments(x => x.Key.Equals(key));
			return count > 0;
		}
	}
}
