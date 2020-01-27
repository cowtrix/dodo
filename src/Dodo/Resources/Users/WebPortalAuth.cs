using Common.Extensions;
using REST.Security;
using Newtonsoft.Json;
using REST;
using System;
using Common.Security;

namespace Dodo.Users
{
	public class WebPortalAuth : IVerifiable
	{
		public WebPortalAuth() { }

		public WebPortalAuth(string userName, string password)
		{
			if(!ValidationExtensions.IsStrongPassword(password, out var error))
			{
				throw new Exception(error);
			}
			Username = userName;
			var passphrase = KeyGenerator.GetUniqueKey(128);
			PassphraseHash = SHA256Utility.SHA256(passphrase);
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
		public string Username { get; set; }

		[View(EPermissionLevel.USER)]
		[JsonProperty]
		public string PublicKey { get; private set; }

		/// <summary>
		/// Am SHA hash of the user's password which we can challenge against
		/// </summary>
		[JsonProperty]
		public string PasswordHash { get; private set; }
		public string PassphraseHash { get; private set; }
		[JsonProperty]
		public EncryptedStore<string> PassPhrase;
		[JsonProperty]
		public EncryptedStore<string> PrivateKey;

		public bool ChallengePassword(string password, out Passphrase passphrase)
		{
			passphrase = default;
			if (SHA256Utility.SHA256(password + PublicKey) != PasswordHash)
			{
				return false;
			}
			if(!PassPhrase.IsAuthorised(null, new Passphrase(password)))
			{
				return false;
			}
			passphrase = new Passphrase(PassPhrase.GetValue(password));
			return true;
		}

		public void ChangePassword(Passphrase oldValue, Passphrase newValue)
		{
			if(!ChallengePassword(oldValue.Value, out var passphrase))
			{
				throw HttpException.FORBIDDEN;
			}
			PassPhrase = new EncryptedStore<string>(passphrase.Value, newValue);
			PasswordHash = SHA256Utility.SHA256(newValue.Value + PublicKey);
			if(!ChallengePassword(newValue.Value, out _))
			{
				throw new Exception("Failed to change password");
			}
		}

		public bool CanVerify()
		{
			return true;
		}
	}
}
