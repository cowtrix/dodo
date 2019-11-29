using SimpleHttpServer.REST;
using System.Security.Authentication;

namespace Common.Security
{
	public struct Passphrase
	{
		private string m_token;
		private string m_data;

		public Passphrase(string value)
		{
			m_token = TemporaryTokenManager.GetTemporaryToken();
			m_data = SymmetricSecurity.Encrypt(value, m_token);
		}

		public string Value
		{
			get
			{
				if(!TemporaryTokenManager.IsValidToken(m_token))
				{
					throw new AuthenticationException("Token Expired or Invalid");
				}
				return SymmetricSecurity.Decrypt<string>(m_data, m_token);
			}
		}
	}
}