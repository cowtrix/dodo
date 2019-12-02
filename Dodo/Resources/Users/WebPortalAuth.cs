using Common;
using Common.Extensions;
using Common.Security;
using Newtonsoft.Json;
using SimpleHttpServer.REST;
using System;
using System.Security.Cryptography;

namespace Dodo.Users
{
	public class WebPortalAuth : IVerifiable
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

		public WebPortalAuth() { }

		public WebPortalAuth(string userName, string password)
		{
			if(!ValidationExtensions.IsStrongPassword(password, out var error))
			{
				throw new Exception(error);
			}
			Username = userName;
			var passphrase = KeyGenerator.GetUniqueKey(128);
			PassPhrase = new EncryptedStore<string>(passphrase, new Passphrase(password));
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			PrivateKey = new EncryptedStore<string>(pv, new Passphrase(passphrase));
			PublicKey = pk;

			// Salt with public key
			PasswordHash = SHA256Utility.SHA256(password + pk);
		}

		/// <summary>
		/// The user's username
		/// </summary>
		[View(EPermissionLevel.USER)]
		[Username]
		[JsonProperty]
		public string Username { get; private set; }

		[View(EPermissionLevel.USER)]
		[JsonProperty]
		public string PublicKey { get; private set; }

		/// <summary>
		/// Am MD5 hash of the user's password which we can challenge against
		/// </summary>
		[JsonProperty]
		public string PasswordHash { get; private set; }

		public ResetToken PasswordResetToken;

		[JsonProperty]
		public EncryptedStore<string> PassPhrase;
		[JsonProperty]
		public EncryptedStore<string> PrivateKey;

		public bool Challenge(string password, out Passphrase passphrase)
		{
			if(SHA256Utility.SHA256(password + PublicKey) != PasswordHash)
			{
				return false;
			}
			passphrase = new Passphrase(PassPhrase.GetValue(password));
			return true;
		}
	}
}
