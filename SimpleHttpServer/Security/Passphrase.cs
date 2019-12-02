using SimpleHttpServer.REST;
using System.Security.Authentication;

namespace Common.Security
{
	/// <summary>
	/// This represents a passphrase - the key of every user that unlocks all of their data.
	/// Each passphrase is short-lived and will stop working after a few minutes (defined by
	/// TemporaryTokenManager.TOKEN_TIMEOUT_MINUTES). The purpose of this is to prevent
	/// long-term or plaintext storage of passphrase strings.
	/// Only expose the value of this object when absolutely necessary!
	/// </summary>
	public struct Passphrase
	{
		private string m_tokenKey;
		private string m_data;

		public Passphrase(string value)
		{
			TemporaryTokenManager.GetTemporaryToken(out var tokenKey, out var token);
			m_tokenKey = tokenKey;
			m_data = SymmetricSecurity.Encrypt(value, token);
		}

		public string Value
		{
			get
			{
				if(string.IsNullOrEmpty(m_tokenKey))
				{
					return null;
				}
				if(!TemporaryTokenManager.IsValidToken(m_tokenKey, out var token))
				{
					throw new AuthenticationException("Token Expired or Invalid");
				}
				return SymmetricSecurity.Decrypt<string>(m_data, token);
			}
		}
	}
}