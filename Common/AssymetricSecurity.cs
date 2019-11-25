using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Common
{
	public static class AssymetricSecurity
	{
		public static void GeneratePublicPrivateKeyPair(out string privateKey, out string publicKey)
		{
			using (var rsa = new RSACryptoServiceProvider())
			{
				privateKey = rsa.ToXmlString(true);
				publicKey = rsa.ToXmlString(false);
			}
		}

		public static byte[] Encrypt<T>(T obj, string publicKey)
		{
			var json = JsonConvert.SerializeObject(obj);
			using (var rsa = new RSACryptoServiceProvider())
			{
				rsa.FromXmlString(publicKey);
				return rsa.Encrypt(json.ToByteArray(), false);
			}
		}

		public static T Decrypt<T>(byte[] data, string privateKey)
		{
			using (var rsa = new RSACryptoServiceProvider())
			{
				rsa.FromXmlString(privateKey);
				var json = rsa.Decrypt(data, false).ByteArrayToString();
				return JsonConvert.DeserializeObject<T>(json);
			}
		}
	}
}
