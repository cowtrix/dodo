using Newtonsoft.Json;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common.Security
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
		private string m_key;
		[JsonProperty]
		private MultiSigEncryptedStore<T, string> m_data;

		public MultiSigKeyStore(T key, string passphrase)
		{
			m_key = KeyGenerator.GetUniqueKey(128);
			m_data = new MultiSigEncryptedStore<T, string>(m_key, key, passphrase);
		}

		public bool TryGetValue(object requester, string passphrase, out object result)
		{
			if(!m_data.TryGetValue(requester, passphrase, out var key))
			{
				result = false;
			}
			else
			{
				result = (key as string) == m_key;
			}
			return (bool)result;
		}

		public void AddPermission(T key, string ownerPass, T newKey, string newUserPass)
		{
			m_data.AddPermission(key, ownerPass, newKey, newUserPass);
		}

		public void SetValue(object innerObject, EUserPriviligeLevel view, object requester, string passphrase)
		{
			throw new NotImplementedException();
		}

		public bool GetValue(T key, string password)
		{
			return m_data.GetValue(key, password) == m_key;
		}
	}
}
