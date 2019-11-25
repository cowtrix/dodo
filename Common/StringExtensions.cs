using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Common
{

	public static class StringExtensions
	{
		public static byte[] ToByteArray(this string str)
		{
			return System.Text.Encoding.ASCII.GetBytes(str);
		}
		public static string ByteArrayToString(this byte[] byteArray)
		{
			return System.Text.Encoding.UTF8.GetString(byteArray);
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
			return System.Net.WebUtility.UrlEncode(Regex.Replace(s.ToLower(), @"\s+", ""));
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
			return new string(Enumerable.Repeat(chars, length)
			  .Select(s => s[random.Next(s.Length)]).ToArray());
		}
	}
}
