using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace Common.Extensions
{

	public static class StringExtensions
	{
		const string fullLinkOnlyRegex = @"\[([^\[]+)\]\((.*?)\)";

		public static string EncodeBase64(this string value)
		{
			var valueBytes = Encoding.UTF8.GetBytes(value);
			return Convert.ToBase64String(valueBytes);
		}

		public static string DecodeBase64(this string value)
		{
			var valueBytes = System.Convert.FromBase64String(value);
			return Encoding.UTF8.GetString(valueBytes);
		}

		public static string ReplaceAll(this string str, IDictionary<string, string> dir)
		{
			foreach(var kvp in dir)
			{
				str = str.Replace(kvp.Key, kvp.Value);
			}
			return str;
		}

		public static string TextToHtml(string str)
		{
			if(string.IsNullOrEmpty(str))
			{
				return str;
			}
			// Firstly escape any explicit html to prevent injection
			str = System.Web.HttpUtility.HtmlEncode(str);
			// While this is nice, we want quotation marks to work
			str = str.Replace("&quot;", "\"");
			// Now format markdown urls
			Match match = Regex.Match(str, fullLinkOnlyRegex);
			while (match.Success)
			{
				// Handle match here...
				try
				{
					var txt = match.Groups[1].Value;
					var link = match.Groups[2].Value;
					str = str.Substring(0, match.Index) + $"<a href=\"{link}\">{txt}</a>" + str.Substring(match.Index + match.Length);
				}
				catch { }
				match = Regex.Match(str, fullLinkOnlyRegex);
			}
			// Finally replace linebreaks with <br/>
			return str
				.Replace(Environment.NewLine, "</br>")
				.Replace("\n", "</br>")
				.Replace("\t", "&emsp;");
		}
		public static string StripMDLinks(string str, bool stripNewlines)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			// Firstly escape any explicit html to prevent injection
			str = System.Web.HttpUtility.HtmlEncode(str);
			// While this is nice, we want quotation marks to work
			str = str.Replace("&quot;", "\"");
			// Now format markdown urls
			Match match = Regex.Match(str, fullLinkOnlyRegex);
			while (match.Success)
			{
				// Handle match here...
				try
				{
					var txt = match.Groups[1].Value;
					var link = match.Groups[2].Value;
					str = str.Substring(0, match.Index) + $": {link}" + str.Substring(match.Index + match.Length);
				}
				catch { }
				match = Regex.Match(str, fullLinkOnlyRegex);
			}
			// Finally replace linebreaks with <br/>
			if(stripNewlines)
			{
				str = str
				.Replace(Environment.NewLine, "")
				.Replace("\n", "");
			}
			return str;
		}
		public static string AppendIfNotNull(this string str, string toAppend)
		{
			if(str == null)
			{
				return str;
			}
			return str + toAppend;
		}
		public static string ToCamelCase(this string str)
		{
			if (string.IsNullOrEmpty(str))
			{
				return str;
			}
			return $"{char.ToLowerInvariant(str[0])}{str.Substring(1)}";
		}
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
			return System.Net.WebUtility.UrlEncode(Regex.Replace(s.ToLower(), @"[^a-zA-Z_\d\s:]", ""));
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
