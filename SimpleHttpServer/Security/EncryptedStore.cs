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
		private string m_encryptedData;
		private string m_passHash;

		public EncryptedStore() { }

		public EncryptedStore(T value, string password)
		{
			if(value != default)
			{
				SetValue(value, password);
			}
		}

		public T GetValue(string password)
		{
			if(string.IsNullOrEmpty(m_encryptedData))
			{
				return default;
			}
			return SymmetricSecurity.Decrypt<T>(m_encryptedData, password);
		}

		public bool IsAuthorised(object requester, string passphrase)
		{
			return string.IsNullOrEmpty(m_passHash) || SHA256Utility.SHA256(passphrase) == m_passHash;
		}

		public void SetValue(T value, string passphrase)
		{
			if(!IsAuthorised(null, passphrase))
			{
				throw new AuthenticationException();
			}
			if(value == default)
			{
				m_encryptedData = null;
			}
			else
			{
				m_encryptedData = SymmetricSecurity.Encrypt(value, passphrase);
			}
			m_passHash = SHA256Utility.SHA256(passphrase);
		}

		public void SetValue(object innerObject, EPermissionLevel view, object requester, string passphrase)
		{
			var data = GetValue(passphrase);
			try
			{
				if (data == default)
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

		public bool TryGetValue(object requester, string password, out object result)
		{
			try
			{
				result = GetValue(password);
				return true;
			}
			catch { }
			result = null;
			return false;
		}
	}
}