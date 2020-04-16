using System;
using System.Text;
using System.Security.Cryptography;
using System.IO;
using System.Linq;
using Newtonsoft.Json;
using Common.Extensions;

namespace Common.Security
{
	/// <summary>
	/// This class provides utilties to symmetrically encrypt information
	/// </summary>
	public static class SymmetricSecurity
	{
		// This constant is used to determine the keysize of the encryption algorithm in bits.
		// We divide this by 8 within the code below to get the equivalent number of bytes.
		private const int KeySize = 256;

		private const int BlockSize = 128;

		// This constant determines the number of iterations for the password bytes generation function.
		private const int DerivationIterations = 1000;

		public static string Encrypt<T>(T objectToEncrypt, string passphrase)
		{
			var plainText = JsonConvert.SerializeObject(objectToEncrypt, JsonExtensions.StorageSettings);
			// Salt and IV is randomly generated each time, but is preprended to encrypted cipher text
			// so that the same Salt and IV values can be used when decrypting.
			var saltStringBytes = GenerateBitsOfRandomEntropy(KeySize);
			var ivStringBytes = GenerateBitsOfRandomEntropy(BlockSize);
			var plainTextBytes = Encoding.UTF8.GetBytes(plainText);
			using (var password = new Rfc2898DeriveBytes(passphrase, saltStringBytes, DerivationIterations))
			{
				var keyBytes = password.GetBytes(KeySize / 8);
				using (var symmetricKey = new RijndaelManaged())
				{
					symmetricKey.BlockSize = BlockSize;
					symmetricKey.Mode = CipherMode.CBC;
					symmetricKey.Padding = PaddingMode.PKCS7;
					using (var encryptor = symmetricKey.CreateEncryptor(keyBytes, ivStringBytes))
					{
						using (var memoryStream = new MemoryStream())
						{
							using (var cryptoStream = new CryptoStream(memoryStream, encryptor, CryptoStreamMode.Write))
							{
								cryptoStream.Write(plainTextBytes, 0, plainTextBytes.Length);
								cryptoStream.FlushFinalBlock();
								// Create the final bytes as a concatenation of the random salt bytes, the random iv bytes and the cipher bytes.
								var cipherTextBytes = saltStringBytes;
								cipherTextBytes = cipherTextBytes.Concat(ivStringBytes).ToArray();
								cipherTextBytes = cipherTextBytes.Concat(memoryStream.ToArray()).ToArray();
								memoryStream.Close();
								cryptoStream.Close();
								return Convert.ToBase64String(cipherTextBytes);
							}
						}
					}
				}
			}
		}

		public static T Decrypt<T>(string cipherText, string passphrase)
		{
			var headerSize = (KeySize / 8) + (BlockSize / 8);
			// Get the complete stream of bytes that represent:
			// [32 bytes of Salt] + [32 bytes of IV] + [n bytes of CipherText]
			var cipherTextBytesWithSaltAndIv = Convert.FromBase64String(cipherText);
			// Get the saltbytes by extracting the first 32 bytes from the supplied cipherText bytes.
			var saltStringBytes = cipherTextBytesWithSaltAndIv.Take(KeySize / 8).ToArray();
			// Get the IV bytes by extracting the next 32 bytes from the supplied cipherText bytes.
			var ivStringBytes = cipherTextBytesWithSaltAndIv.Skip(KeySize / 8).Take(BlockSize / 8).ToArray();
			// Get the actual cipher text bytes by removing the first 64 bytes from the cipherText string.
			var cipherTextBytes = cipherTextBytesWithSaltAndIv.Skip(headerSize).Take(cipherTextBytesWithSaltAndIv.Length - (headerSize)).ToArray();

			using (var password = new Rfc2898DeriveBytes(passphrase, saltStringBytes, DerivationIterations))
			{
				var keyBytes = password.GetBytes(KeySize / 8);
				using (var symmetricKey = new RijndaelManaged())
				{
					symmetricKey.BlockSize = BlockSize;
					symmetricKey.Mode = CipherMode.CBC;
					symmetricKey.Padding = PaddingMode.PKCS7;
					using (var decryptor = symmetricKey.CreateDecryptor(keyBytes, ivStringBytes))
					{
						using (var memoryStream = new MemoryStream(cipherTextBytes))
						{
							using (var cryptoStream = new CryptoStream(memoryStream, decryptor, CryptoStreamMode.Read))
							{
								var plainTextBytes = new byte[cipherTextBytes.Length];
								var decryptedByteCount = cryptoStream.Read(plainTextBytes, 0, plainTextBytes.Length);
								memoryStream.Close();
								cryptoStream.Close();
								var plainText = Encoding.UTF8.GetString(plainTextBytes, 0, decryptedByteCount);
								/*var obj = JsonConvert.DeserializeObject(plainText, JsonExtensions.StorageSettings);
								if(obj is Newtonsoft.Json.Linq.JObject jobj)
								{
									return jobj.ToObject<T>();
								}*/
								return JsonConvert.DeserializeObject<T>(plainText, JsonExtensions.StorageSettings);
							}
						}
					}
				}
			}
		}

		private static byte[] GenerateBitsOfRandomEntropy(int amount)
		{
			var randomBytes = new byte[amount / 8];
			using (var rngCsp = new RNGCryptoServiceProvider())
			{
				// Fill the array with cryptographically secure random bytes.
				rngCsp.GetBytes(randomBytes);
			}
			return randomBytes;
		}
	}
}
