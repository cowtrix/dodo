using Newtonsoft.Json;
using System.Collections.Concurrent;
using System.Collections.Generic;

namespace Common
{
	public class EncryptedStore<T> : IDecryptable<T>
	{
		[JsonProperty]
		private string m_encryptedData;

		public EncryptedStore(T value, string password)
		{
			if(value != default)
			{
				m_encryptedData = StringCipher.Encrypt(JsonConvert.SerializeObject(value), password);
			}
		}

		public T GetValue(string password)
		{
			if(string.IsNullOrEmpty(m_encryptedData))
			{
				return default;
			}
			return JsonConvert.DeserializeObject<T>(StringCipher.Decrypt(m_encryptedData, password));
		}

		public void SetValue(T value, string password)
		{
			GetValue(password); // Will throw exception if incorrect password
			if(value == default)
			{
				m_encryptedData = null;
			}
			else
			{
				m_encryptedData = StringCipher.Encrypt(JsonConvert.SerializeObject(value), password);
			}
		}

		public void SetValue(object innerObject, object requester, string passphrase)
		{
			var data = GetValue(passphrase);
			try
			{
				data = data.PatchObject(innerObject as Dictionary<string, object>, requester, passphrase);
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