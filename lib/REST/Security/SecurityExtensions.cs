using Common.Security;
using System;

namespace Resources.Security
{
	public static class SecurityExtensions
	{
		/// <summary>
		/// It's important that we don't store the keys directly, so we instead create a one-way hash of the
		/// key and passphrase. Note that then when we change a key's passphrase, we must remove the old hash.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="passphrase"></param>
		/// <returns></returns>
		public static string GenerateID(object key, Passphrase passphrase, string salt)
		{
			return GenerateID(key, passphrase.Value, salt);
		}

		/// <summary>
		/// It's important that we don't store the keys directly, so we instead create a one-way hash of the
		/// key and passphrase. Note that then when we change a key's passphrase, we must remove the old hash.
		/// </summary>
		/// <param name="key"></param>
		/// <param name="passphrase"></param>
		/// <returns></returns>
		public static string GenerateID(object key, string passphrase, string salt)
		{
			if(key == null)
			{
				return string.Empty;
			}
			if(string.IsNullOrEmpty(salt))
			{
				throw new ArgumentNullException(nameof(salt));
			}
			return SHA256Utility.SHA256(key.GetHashCode() + passphrase + salt);
		}
	}
}
