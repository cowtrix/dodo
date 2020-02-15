using Common.Security;

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
		public static string GenerateID(object key, Passphrase passphrase)
		{
			return SHA256Utility.SHA256(key.GetHashCode() + passphrase.Value);
		}
	}
}