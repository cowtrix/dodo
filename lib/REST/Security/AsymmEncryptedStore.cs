using Common.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Security.Authentication;

namespace Resources.Security
{
	/// <summary>
	/// Asymmetrically stores an object of type T in an encrypted form of its JSON representation
	/// </summary>
	/// <typeparam name="T"></typeparam>
	public class AsymmEncryptedStore<T> : IDecryptable<T>
	{
		const int keySize = 64;

		[BsonElement]
		[JsonProperty]
		private byte[] m_token;
		[BsonElement]
		[JsonProperty]
		private string m_encryptedData;
		[BsonElement]
		[JsonProperty]
		private string m_passHash;

		public AsymmEncryptedStore() { }

		public AsymmEncryptedStore(T value, Passphrase publicKey)
		{
			if (!value.Equals(default))
			{
				SetValue(value, publicKey);
			}
		}

		public T GetValue(Passphrase privateKey)
		{
			return GetValue(privateKey.Value);
		}

		public T GetValue(string privateKey)
		{
			if (string.IsNullOrEmpty(m_encryptedData))
			{
				return default;
			}
			var token = AsymmetricSecurity.Decrypt<string>(m_token, privateKey);
			return SymmetricSecurity.Decrypt<T>(m_encryptedData, token);
		}

		public bool IsAuthorised(object requester, Passphrase passphrase)
		{
			return string.IsNullOrEmpty(m_passHash) || SHA256Utility.SHA256(passphrase.Value) == m_passHash;
		}

		public void SetValue(T value, Passphrase publicKey)
		{
			if (!IsAuthorised(null, publicKey))
			{
				throw new AuthenticationException();
			}
			if (value.Equals(default))
			{
				m_token = null;
				m_encryptedData = null;
				m_passHash = null;
			}
			else
			{
				var rawToken = KeyGenerator.GetUniqueKey(keySize) + value.GetHashCode().ToString();
				m_token = AsymmetricSecurity.Encrypt(rawToken, publicKey.Value);
				m_encryptedData = SymmetricSecurity.Encrypt(value, rawToken);
				m_passHash = SHA256Utility.SHA256(rawToken);
			}
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
