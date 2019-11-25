using System.Text;
using System.Security.Cryptography;

namespace Common
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
			byte[] data = Encoding.ASCII.GetBytes(plaintext);
			using (var sha256 = new SHA256Managed())
			{
				data = sha256.ComputeHash(data);
			}
			return Encoding.ASCII.GetString(data);
		}
	}
}