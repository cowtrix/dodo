using System;
using System.Linq;
using System.Security.Cryptography;
using System.Text;

namespace Common
{
	public static class KeyGenerator
	{
		internal static readonly char[] chars =
			"abcdefghijklmnopqrstuvwxyzABCDEFGHIJKLMNOPQRSTUVWXYZ1234567890".ToCharArray();

		/// <summary>
		/// Generate a cryptographically secure random key of length 'size'
		/// </summary>
		/// <param name="size"></param>
		/// <returns></returns>
		public static string GetUniqueKey(int size)
		{
			byte[] data = new byte[4 * size];
			using (RNGCryptoServiceProvider crypto = new RNGCryptoServiceProvider())
			{
				crypto.GetBytes(data);
			}
			StringBuilder result = new StringBuilder(size);
			for (int i = 0; i < size; i++)
			{
				var rnd = BitConverter.ToUInt32(data, i * 4);
				var idx = rnd % chars.Length;

				result.Append(chars[idx]);
			}

			return result.ToString();
		}
	}
}
