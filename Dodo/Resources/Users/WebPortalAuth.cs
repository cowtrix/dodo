using Common;
using Common.Security;
using Newtonsoft.Json;
using SimpleHttpServer.REST;
using System;
using System.Security.Cryptography;

namespace Dodo.Users
{
	public struct WebPortalAuth
	{
		public struct ResetToken
		{
			const int TimeoutMinutes = 15;
			[JsonProperty]
			private string m_token;
			[JsonProperty]
			private DateTime m_timestamp;

			public static ResetToken Generate()
			{
				return new ResetToken()
				{
					m_token = KeyGenerator.GetUniqueKey(64),
					m_timestamp = DateTime.Now,
				};
			}
		}

		public WebPortalAuth(string userName, string password) : this()
		{
			Username = userName;
			PasswordHash = SHA256Utility.SHA256(password);
			var passphrase = KeyGenerator.GetUniqueKey(128);
			PassPhrase = new EncryptedStore<string>(passphrase, password);
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			PrivateKey = new EncryptedStore<string>(pv, passphrase);
			PublicKey = pk;
		}

		/// <summary>
		/// The user's username
		/// </summary>
		[View(EPermissionLevel.USER)]
		[Username]
		public string Username { get; private set; }

		[View(EPermissionLevel.USER)]
		public string PublicKey { get; private set; }

		/// <summary>
		/// Am MD5 hash of the user's password
		/// </summary>
		public string PasswordHash { get; private set; }

		public ResetToken PasswordResetToken;

		public EncryptedStore<string> PassPhrase;
		public EncryptedStore<string> PrivateKey;

		public bool Challenge(string password, out string passphrase)
		{
			if(SHA256Utility.SHA256(password) != PasswordHash)
			{
				passphrase = null;
				return false;
			}
			passphrase = PassPhrase.GetValue(password);
			return true;
		}
	}
}
