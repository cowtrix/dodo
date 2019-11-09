using Newtonsoft.Json;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common
{
	/// <summary>
	/// This class allows multiple key/password combinations to be able to decrypt a single piece of data.
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TVal"></typeparam>
	public class MultiSigEncryptedStore<TKey, TVal> : IKeyDecryptable<TKey,TVal>
	{
		/// <summary>
		/// Here we store keys to a common encrypted passphrase that can be used to decrypt the data.
		/// A key, corresponding with a valid password, will give the common passphrase that can
		/// be used to decrypt the data.
		/// </summary>
		[JsonProperty]
		private ConcurrentDictionary<TKey, EncryptedStore<string>> m_keyStore = new ConcurrentDictionary<TKey, EncryptedStore<string>>();
		[JsonProperty]
		private EncryptedStore<TVal> m_data;

		public MultiSigEncryptedStore() { }

		public MultiSigEncryptedStore(TVal data, TKey key, string password)
		{
			var passPhrase = SHA256Utility.SHA256(Guid.NewGuid().ToString() + key.GetHashCode().ToString() + data?.GetHashCode());	// Generate a passphrase
			m_keyStore.TryAdd(key, new EncryptedStore<string>(passPhrase, password)); // Store the creating key and the passphrase with the given password
			m_data = new EncryptedStore<TVal>(data, passPhrase);	// Encrypt the common data with the common passphrase
		}

		public TVal GetValue(TKey key, string password)
		{
			if(!m_keyStore.TryGetValue(key, out var unlockPhrase))
			{
				throw new Exception("You are not authorized to access this resource");
			}
			var passPhrase = unlockPhrase.GetValue(password);
			return m_data.GetValue(passPhrase);
		}

		public void SetValue(TVal data, TKey key, string password)
		{
			if (!m_keyStore.TryGetValue(key, out var unlockPhrase))
			{
				throw new Exception("You are not authorized to access this resource");
			}
			var passPhrase = unlockPhrase.GetValue(password);
			m_data = new EncryptedStore<TVal>(data, passPhrase);
		}

		public bool IsAuthorised(TKey key)
		{
			return m_keyStore.ContainsKey(key);
		}

		public void AddPermission(TKey key, string ownerPass, TKey newKey, string newUserPass)
		{
			if (!m_keyStore.TryGetValue(key, out var unlockPhrase))
			{
				throw new Exception("You are not authorized to access this resource");
			}
			var passPhrase = unlockPhrase.GetValue(ownerPass);
			m_keyStore.TryAdd(newKey, new EncryptedStore<string>(passPhrase, newUserPass));
		}

		public bool TryGetValue(object requester, string password, out object result)
		{
			try
			{
				result = GetValue((TKey)requester, password);
				return true;
			}
			catch { }
			result = null;
			return false;
		}

		public void SetValue(object innerObject, object requester, string passphrase)
		{
			var data = GetValue((TKey)requester, passphrase);
			try
			{
				data = data.PatchObject(innerObject as Dictionary<string, object>, requester, passphrase);
				SetValue(data, (TKey)requester, passphrase);
				return;
			}
			catch { }
			SetValue((TVal)innerObject, (TKey)requester, passphrase);
		}
	}
}
