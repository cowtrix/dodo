using System.Text;
using System.Security.Cryptography;
using Common.Extensions;
using System;

namespace Common.Security
{
	public static class SHA256Utility
	{
		/// <summary>
		/// Creates a SHA256 hash of a given plaintext
		/// </summary>
		/// <param name="plaintext"></param>
		/// <returns></returns>
		public static string SHA256(string plaintext)
		{
			if(string.IsNullOrEmpty(plaintext))
			{
				return null;
			}
			byte[] data = Encoding.ASCII.GetBytes(plaintext);
			using (var sha256 = new SHA256Managed())
			{
				var rawBytes = sha256.ComputeHash(data);
				return BitConverter.ToString(rawBytes);
			}
		}
	}
}