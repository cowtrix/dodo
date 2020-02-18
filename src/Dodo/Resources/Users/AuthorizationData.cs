using Common.Extensions;
using Common.Security;
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
			// URGENT TODO: we can't really use the password here
			// BECAUSE we need to change the key to the passphrase when the user auth changes
			PasswordHash = HashPassword(password);
			Username = userName;
			var passphrase = KeyGenerator.GetUniqueKey(128);
			PassphraseHash = SHA256Utility.SHA256(passphrase);
			PassPhrase = new EncryptedStore<string>(passphrase, new Passphrase(password));
			AsymmetricSecurity.GeneratePublicPrivateKeyPair(out var pv, out var pk);
			PrivateKey = new EncryptedStore<string>(pv, new Passphrase(passphrase));
			PublicKey = pk;
			SecurityStamp = SHA256Utility.SHA256(pv + password);
		}

		private static string HashPassword(string password)
		{
			byte[] salt;
			byte[] buffer2;
			if (password == null)
			{
				throw new ArgumentNullException("password");
			}
			using (Rfc2898DeriveBytes bytes = new Rfc2898DeriveBytes(password, 0x10, 0x3e8))
			{
				salt = bytes.Salt;
				buffer2 = bytes.GetBytes(0x20);
			}
			byte[] dst = new byte[0x31];
			Buffer.BlockCopy(salt, 0, dst, 1, 0x10);
			Buffer.BlockCopy(buffer2, 0, dst, 0x11, 0x20);
			return Convert.ToBase64String(dst);
		}

		public bool ChallengePassword(string password, out string passphrase)
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
			passphrase = PassPhrase.GetValue(password);
			return true;
		}

		public bool ChangePassword(Passphrase oldValue, Passphrase newValue)
		{
			if(!ChallengePassword(oldValue.Value, out var passphrase))
			{
				return false;
			}
			PassPhrase = new EncryptedStore<string>(passphrase, newValue);
			PasswordHash = SHA256Utility.SHA256(newValue.Value + PublicKey);
			if(!ChallengePassword(newValue.Value, out _))
			{
				return false;
			}
			return true;
		}
		
		public bool CanVerify()
		{
			return true;
		}
	}
}
