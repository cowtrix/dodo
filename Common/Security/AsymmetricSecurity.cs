using Newtonsoft.Json;
using System.Security.Cryptography;

namespace Common
{

	/// <summary>
	/// Wrapper for RSACryptoServiceProvider to handle asymmetrical encryption
	/// </summary>
	public static class AsymmetricSecurity
	{
		/// <summary>
		/// Generate a new private/public key pair as XML strings
		/// </summary>
		/// <param name="privateKey"></param>
		/// <param name="publicKey"></param>
		public static void GeneratePublicPrivateKeyPair(out string privateKey, out string publicKey)
		{
			using (var rsa = new RSACryptoServiceProvider())
			{
				privateKey = rsa.ToXmlString(true);
				publicKey = rsa.ToXmlString(false);
			}
		}

		/// <summary>
		/// Serialize an object as JSON and then encrypt it to a byte array with the given public key
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="objectToEncrypt"></param>
		/// <param name="publicKey"></param>
		/// <returns></returns>
		public static byte[] Encrypt<T>(T objectToEncrypt, string publicKey)
		{
			var json = JsonConvert.SerializeObject(objectToEncrypt);
			using (var rsa = new RSACryptoServiceProvider())
			{
				rsa.FromXmlString(publicKey);
				return rsa.Encrypt(json.ToByteArray(), false);
			}
		}

		/// <summary>
		/// Decrypt a byte array with a given private key, and then deserialize the resulting JSON into an object
		/// of type T
		/// </summary>
		/// <typeparam name="T"></typeparam>
		/// <param name="data"></param>
		/// <param name="privateKey"></param>
		/// <returns></returns>
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
