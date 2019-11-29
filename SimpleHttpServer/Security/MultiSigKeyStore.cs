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

		public MultiSigKeyStore(T key, Passphrase passphrase)
		{
			m_key = KeyGenerator.GetUniqueKey(128);
			m_data = new MultiSigEncryptedStore<T, string>(m_key, key, passphrase);
		}

		public void AddPermission(T key, Passphrase ownerPass, T newKey, Passphrase newUserPass)
		{
			m_data.AddPermission(key, ownerPass, newKey, newUserPass);
		}

		public bool IsAuthorised(T key, Passphrase password)
		{
			return m_data.GetValue(key, password) == m_key;
		}
	}
}
