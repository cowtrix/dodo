using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using SimpleHttpServer.REST;
using System;
using System.Collections.Concurrent;
using System.Collections.Generic;
using System.Security.Authentication;

namespace Common.Security
{

	/// <summary>
	/// Symmetrically stores an object of type T in an encrypted form of its JSON representation
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class EncryptedStore<T> : IDecryptable<T>
	{
		[JsonProperty]
		[BsonElement]
		private string m_encryptedData;
		[BsonElement]
		[JsonProperty]
		private string m_passHash;

		public EncryptedStore() { }

		public EncryptedStore(T value, Passphrase passphrase)
		{
			if(!value.Equals(default))
			{
				SetValue(value, passphrase);
			}
		}

		public T GetValue(Passphrase passphrase)
		{
			if(string.IsNullOrEmpty(m_encryptedData))
			{
				return default;
			}
			return SymmetricSecurity.Decrypt<T>(m_encryptedData, passphrase.Value);
		}

		public T GetValue(string passphrase)
		{
			if (string.IsNullOrEmpty(m_encryptedData))
			{
				return default;
			}
			return SymmetricSecurity.Decrypt<T>(m_encryptedData, passphrase);
		}

		public bool IsAuthorised(object requester, Passphrase passphrase)
		{
			return string.IsNullOrEmpty(m_passHash) || SHA256Utility.SHA256(passphrase.Value) == m_passHash;
		}

		public void SetValue(T value, Passphrase passphrase)
		{
			if(!IsAuthorised(null, passphrase))
			{
				throw new AuthenticationException();
			}
			if(value.Equals(default))
			{
				m_encryptedData = null;
			}
			else
			{
				m_encryptedData = SymmetricSecurity.Encrypt(value, passphrase.Value);
			}
			m_passHash = SHA256Utility.SHA256(passphrase.Value);
		}

		public void SetValue(object innerObject, EPermissionLevel view, object requester, Passphrase passphrase)
		{
			var data = GetValue(passphrase);
			try
			{
				if (data.Equals(default))
				{
					data = Activator.CreateInstance<T>();
				}
				data = data.PatchObject((Dictionary<string, object>)innerObject, view, requester, passphrase);
				SetValue(data, passphrase);
				return;
			}
			catch { }
			SetValue((T)innerObject, passphrase);
		}

		public bool TryGetValue(object requester, Passphrase passphrase, out object result)
		{
			try
			{
				result = GetValue(passphrase);
				return true;
			}
			catch { }
			result = null;
			return false;
		}
	}
}