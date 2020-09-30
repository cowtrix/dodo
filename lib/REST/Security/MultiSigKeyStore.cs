using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Resources.Security
{
	/// <summary>
	/// This class allows multiple key/value pairs to prove that they have been added to this
	/// collection when they provide a correct passphrase, without an external force being
	/// able to prove their presence within it
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TVal"></typeparam>
	public class MultiSigKeyStore<T>
	{
		[JsonProperty]
		[BsonElement]
		private ConcurrentDictionary<string, byte> m_data = new ConcurrentDictionary<string, byte>();
		[JsonProperty]
		[BsonElement]
		private string Guid { get; set; }

		public MultiSigKeyStore()
		{
			Guid = System.Guid.NewGuid().ToString();
		}

		public int Count
		{
			get { return m_data.Count; }
		}

		public void Add(T key, Passphrase ownerPass)
		{
			var id = SecurityExtensions.GenerateID(key, ownerPass, Guid);
			m_data[id] = 0;
		}

		public bool Remove(T key, Passphrase ownerPass)
		{
			var id = SecurityExtensions.GenerateID(key, ownerPass, Guid);
			return m_data.TryRemove(id, out _);
		}

		public bool IsAuthorised(T key, Passphrase ownerPass)
		{
			if(key.Equals(default) || string.IsNullOrEmpty(ownerPass.Value))
			{
				return false;
			}
			var id = SecurityExtensions.GenerateID(key, ownerPass, Guid);
			return m_data.ContainsKey(id);
		}
	}
}
