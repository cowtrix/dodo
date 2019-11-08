using System.Text;
using System.Security.Cryptography;

namespace Common
{
	public static class SHA256Utility
	{
		public static string SHA256(string plaintext)
		{
			byte[] data = Encoding.ASCII.GetBytes(plaintext);
			data = new SHA256Managed().ComputeHash(data);
			return System.Text.Encoding.ASCII.GetString(data);
		}
	}
}