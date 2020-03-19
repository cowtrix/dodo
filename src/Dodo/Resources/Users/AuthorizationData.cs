using Common.Extensions;
using Common.Security;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using Resources;
using Resources.Security;
using System;
using System.Security.Cryptography;

namespace Dodo.Users
{
	public class AuthorizationData : IVerifiable
	{

		/// <summary>
		///     A random value that must change whenever a users credentials change
		///     (password changed, login removed)
		/// </summary>
		public string SecurityStamp { get; set; }
		public virtual bool TwoFactorEnabled { get; set; }
		public virtual DateTime? LockoutEndDateUtc { get; set; }
		public virtual bool LockoutEnabled { get; set; }
		public virtual int AccessFailedCount { get; set; }

		/// <summary>
		/// The user's username
		/// </summary>
		[View(EPermissionLevel.USER)]
		[Username]
		public string Username { get; set; }

		[View(EPermissionLevel.USER)]
		public string PublicKey { get; set; }

		/// <summary>
		/// Am SHA hash of the user's password which we can challenge against
		/// </summary>
		public string PasswordHash { get; set; }
		public string PassphraseHash { get; set; }
		[JsonProperty]
		public EncryptedStore<string> PassPhrase;
		[JsonProperty]
		public EncryptedStore<string> PrivateKey;

		public AuthorizationData()
		{
			SecurityStamp = "";
		}

		public AuthorizationData(string userName, string password)
		{
			Username = userName;

			var passphrase = KeyGenerator.GetUniqueKey(128);
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			PrivateKey = new EncryptedStore<string>(pv, new Passphrase(passphrase));
			PublicKey = pk;
			PasswordHash = PasswordHasher.HashPassword(password);
			PassphraseHash = PasswordHasher.HashPassword(passphrase);
			PassPhrase = new EncryptedStore<string>(passphrase, new Passphrase(password));
			SecurityStamp = SHA256Utility.SHA256(pv + password);
		}

		public bool ChallengePassword(string password, out string passphrase)
		{
			passphrase = default;
			if (!PasswordHasher.VerifyHashedPassword(PasswordHash, password))
			{
				return false;
			}
			if(!PassPhrase.IsAuthorised(null, new Passphrase(password)))
			{
				return false;
			}
			passphrase = PassPhrase.GetValue(password);
			return true;
		}

		public bool ChangePassword(Passphrase oldPassword, Passphrase newPassword)
		{
			if(!ChallengePassword(oldPassword.Value, out var passphrase))
			{
				return false;
			}
			PassPhrase = new EncryptedStore<string>(passphrase, newPassword);
			PasswordHash = PasswordHasher.HashPassword(newPassword.Value);
			if (!ChallengePassword(newPassword.Value, out _))
			{
				return false;
			}
			var pv = PrivateKey.GetValue(passphrase);
			SecurityStamp = SHA256Utility.SHA256(pv + newPassword.Value);
			return true;
		}
		
		public bool CanVerify()
		{
			return true;
		}
	}
}
