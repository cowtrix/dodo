using Newtonsoft.Json;
using System.Collections.Concurrent;

namespace Common
{
	public struct EncryptedStore<T>
	{
		private string m_encryptedData;

		public EncryptedStore(T value, string password)
		{
			m_encryptedData = StringCipher.Encrypt(JsonConvert.SerializeObject(value), password);
		}

		public T GetValue(string password)
		{
			return JsonConvert.DeserializeObject<T>(StringCipher.Decrypt(m_encryptedData, password));
		}
	}
}