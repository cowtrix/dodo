using Common.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Authentication;

namespace Resources.Security
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
		private class Keystore : ConcurrentDictionary<string, SymmEncryptedStore<string>>
		{
			[JsonConstructor]
			public Keystore() : base() { }
		}

		/// <summary>
		/// Here we store keys to a common encrypted passphrase that can be used to decrypt the data.
		/// A key, corresponding with a valid password, will give the common passphrase that can
		/// be used to decrypt/encrypt the data.
		/// </summary>
		[JsonProperty]
		[BsonElement]
		private Keystore m_keyStore = new Keystore();
		[JsonProperty]
		[BsonElement]
		private SymmEncryptedStore<TVal> m_data;

		public MultiSigEncryptedStore() { }

		public MultiSigEncryptedStore(TVal data, TKey key, Passphrase password)
		{
			var commonKey = new Passphrase(SHA256Utility.SHA256(Guid.NewGuid().ToString() + key.GetHashCode().ToString() + data?.GetHashCode()));	// Generate a passphrase
			m_keyStore.TryAdd(SecurityExtensions.GenerateID(key, password), new SymmEncryptedStore<string>(commonKey.Value, password)); // Store the creating key and the passphrase with the given password
			m_data = new SymmEncryptedStore<TVal>(data, commonKey);	// Encrypt the common data with the common passphrase
		}

		public TVal GetValue(TKey key, Passphrase password)
		{
			if(!m_keyStore.TryGetValue(SecurityExtensions.GenerateID(key, password), out var unlockPhrase))
			{
				throw new AuthenticationException("You are not authorized to access this resource");
			}
			var passPhrase = unlockPhrase.GetValue(password);
			return m_data.GetValue(passPhrase);
		}

		public void SetValue(TVal data, TKey key, Passphrase password)
		{
			if (!m_keyStore.TryGetValue(SecurityExtensions.GenerateID(key, password), out var unlockPhrase))
			{
				throw new AuthenticationException("You are not authorized to access this resource");
			}
			var passPhrase = new Passphrase(unlockPhrase.GetValue(password));
			m_data = new SymmEncryptedStore<TVal>(data, passPhrase);
		}

		public bool IsAuthorised(TKey key, Passphrase passphrase)
		{
			return m_keyStore.ContainsKey(SecurityExtensions.GenerateID(key, passphrase));
		}

		public void AddPermission(TKey key, Passphrase ownerPass, TKey newKey, Passphrase newUserPass)
		{
			if (!m_keyStore.TryGetValue(SecurityExtensions.GenerateID(key, ownerPass), out var unlockPhrase))
			{
				throw new AuthenticationException("You are not authorized to access this resource");
			}
			var passPhrase = unlockPhrase.GetValue(ownerPass);
			if(key.Equals(newKey))
			{
				m_keyStore.TryRemove(SecurityExtensions.GenerateID(key, ownerPass), out _);
			}
			m_keyStore[SecurityExtensions.GenerateID(newKey, newUserPass)] = new SymmEncryptedStore<string>(passPhrase, newUserPass);
		}

		public bool TryGetValue(object requester, Passphrase password, out object result)
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

		public void SetValue(object innerObject, EPermissionLevel view, object requester, Passphrase passphrase)
		{
			var data = GetValue((TKey)requester, passphrase);
			try
			{
				if (data.Equals(default))
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

		public bool IsAuthorised(object requester, Passphrase passphrase)
		{
			return m_keyStore.TryGetValue(SecurityExtensions.GenerateID((TKey)requester, passphrase), out _);
		}
	}
}
