using System;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Extensions
{

	public static class StringExtensions
	{
		public static byte[] AsciiToByteArray(this string str)
		{
			return System.Text.Encoding.ASCII.GetBytes(str);
		}
		public static string ByteArrayToAsciiString(this byte[] byteArray)
		{
			return Encoding.ASCII.GetString(byteArray);
		}
		public static string Base64Decode(string base64EncodedData)
		{
			var base64EncodedBytes = System.Convert.FromBase64String(base64EncodedData);
			return System.Text.Encoding.UTF8.GetString(base64EncodedBytes);
		}
		public static string Base64Encode(string plainText)
		{
			var plainTextBytes = System.Text.Encoding.UTF8.GetBytes(plainText);
			return System.Convert.ToBase64String(plainTextBytes);
		}
		public static string StripForURL(this string s)
		{
			if(string.IsNullOrEmpty(s))
			{
				return s;
			}
			return System.Net.WebUtility.UrlEncode(Regex.Replace(s.ToLower(), @"[^a-zA-Z\d\s:]", ""));
		}

		private static Random random = new Random();

		/// <summary>
		/// Generate a random string of the given length.
		/// WARNING: This function is NOT cryptographically secure
		/// If you are using this for any kind of secure purpose,
		/// DON'T - use KeyGenerator.GetUniqueKey instead
		/// </summary>
		/// <param name="length"></param>
		/// <returns></returns>
		public static string RandomString(int length)
		{
			const string chars = "ABCDEFGHIJKLMNOPQRSTUVWXYZ0123456789";
			lock(random)	// Random is not thread sage
			{
				return new string(Enumerable.Repeat(chars, length)
					.Select(s => s[random.Next(s.Length)]).ToArray());
			}
		}
	}
}
