using Newtonsoft.Json;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common.Security
{
	/// <summary>
	/// This class allows multiple key/password combinations to be able to decrypt a single piece of data.
	/// An external force should not be able to prove which keys would be able to gain access to this data
	/// without the corresponding passphrase
	/// </summary>
	/// <typeparam name="TKey"></typeparam>
	/// <typeparam name="TVal"></typeparam>
	public class MultiSigEncryptedStore<TKey, TVal> : IKeyDecryptable<TKey,TVal>
	{
		/// <summary>
		/// Here we store keys to a common encrypted passphrase that can be used to decrypt the data.
		/// A key, corresponding with a valid password, will give the common passphrase that can
		/// be used to decrypt/encrypt the data.
		/// </summary>
		[JsonProperty]
		private ConcurrentDictionary<string, EncryptedStore<string>> m_keyStore = new ConcurrentDictionary<string, EncryptedStore<string>>();
		[JsonProperty]
		private EncryptedStore<TVal> m_data;

		public MultiSigEncryptedStore() { }

		public MultiSigEncryptedStore(TVal data, TKey key, string password)
		{
			var passPhrase = SHA256Utility.SHA256(Guid.NewGuid().ToString() + key.GetHashCode().ToString() + data?.GetHashCode());	// Generate a passphrase
			m_keyStore.TryAdd(GenerateID(key, password), new EncryptedStore<string>(passPhrase, password)); // Store the creating key and the passphrase with the given password
			m_data = new EncryptedStore<TVal>(data, passPhrase);	// Encrypt the common data with the common passphrase
		}

		public TVal GetValue(TKey key, string password)
		{
			if(!m_keyStore.TryGetValue(GenerateID(key, password), out var unlockPhrase))
			{
				throw new Exception("You are not authorized to access this resource");
			}
			var passPhrase = unlockPhrase.GetValue(password);
			return m_data.GetValue(passPhrase);
		}

		public void SetValue(TVal data, TKey key, string password)
		{
			if (!m_keyStore.TryGetValue(GenerateID(key, password), out var unlockPhrase))
			{
				throw new Exception("You are not authorized to access this resource");
			}
			var passPhrase = unlockPhrase.GetValue(password);
			m_data = new EncryptedStore<TVal>(data, passPhrase);
		}

		public bool IsAuthorised(TKey key, string passphrase)
		{
			return m_keyStore.ContainsKey(GenerateID(key, passphrase));
		}

		public void AddPermission(TKey key, string ownerPass, TKey newKey, string newUserPass)
		{
			if (!m_keyStore.TryGetValue(GenerateID(key, ownerPass), out var unlockPhrase))
			{
				throw new Exception("You are not authorized to access this resource");
			}
			var passPhrase = unlockPhrase.GetValue(ownerPass);
			if(key.Equals(newKey))
			{
				m_keyStore.TryRemove(GenerateID(key, ownerPass), out _);
			}
			m_keyStore[GenerateID(newKey, newUserPass)] = new EncryptedStore<string>(passPhrase, newUserPass);
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

		public void SetValue(object innerObject, EUserPriviligeLevel view, object requester, string passphrase)
		{
			var data = GetValue((TKey)requester, passphrase);
			try
			{
				if (data == default)
				{
					data = Activator.CreateInstance<TVal>();
				}
				data = data.PatchObject((Dictionary<string, object>)innerObject, view, requester, passphrase);
				SetValue(data, (TKey)requester, passphrase);
				return;
			}
			catch { }
			SetValue((TVal)innerObject, (TKey)requester, passphrase);
		}

		/// <summary>
		/// It's important that we don't store the keys directly, so we instead create a one-way hash of the
		/// key and passphrase. Note that then when we change a key's passphrase, we must remove the old hash.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="passphrase"></param>
		/// <returns></returns>
		private string GenerateID(TKey key, string passphrase)
		{
			return SHA256Utility.SHA256(key.GetHashCode() + passphrase);
		}
	}
}
