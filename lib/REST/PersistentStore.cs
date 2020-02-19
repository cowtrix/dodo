using MongoDB.Bson.Serialization.Attributes;
using MongoDB.Driver;
using MongoDB.Driver.Linq;
using Newtonsoft.Json;
using System;
using System.Collections;
using System.Collections.Generic;
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

		IMongoCollection<Entry> m_collection;
		public PersistentStore(string dbName, string collectionName)
		{
			m_collection = ResourceUtility.MongoDB.GetDatabase(dbName).GetCollection<Entry>(collectionName);
			var indexOptions = new CreateIndexOptions();
			var indexKeys = Builders<Entry>.IndexKeys
				.Ascending(rsc => rsc.Key);
			var indexModel = new CreateIndexModel<Entry>(indexKeys, indexOptions);
			m_collection.Indexes.CreateOne(indexModel);
		}

		public TValue this[TKey key]    // Indexer declaration  
		{
			get
			{
				return m_collection.Find(x => x.Key != null && x.Key.Equals(key)).First().Value;
			}
			set
			{
				m_collection.InsertOne(new Entry(key, value));
				//m_collection.ReplaceOne(x => x.Key != null && x.Key.Equals(key), new Entry(key, JsonConvert.SerializeObject(value)));
			}
		}

		public IMongoQueryable<Entry> GetQueryable()
		{
			return m_collection.AsQueryable();
		}

		public bool Remove(TKey key)
		{
			return m_collection.DeleteOne(x => x.Key.Equals(key)).IsAcknowledged;
		}

		public bool TryGetValue(TKey key, out TValue value)
		{
			var match = m_collection.Find(x => x.Key.Equals(key));
			if(!match.Any())
			{
				value = default;
				return false;
			}
			value = match.First().Value;
			return true;
		}
	}
}
