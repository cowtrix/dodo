using MongoDB.Driver;
using System;
using System.Collections.Generic;
using System.Text;

namespace Resources
{
	public class PersistentStore<TKey, TValue>
	{
		struct Entry
		{
			public TKey Key;
			public TValue Object;
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
				return m_collection.Find(x => x.Key != null && x.Key.Equals(key)).First().Object;
			}
			set
			{
				m_collection.FindOneAndUpdate(x => x.Key != null && x.Key.Equals(key), new ObjectUpdateDefinition<Entry>(value));
			}
		}
	}
}
