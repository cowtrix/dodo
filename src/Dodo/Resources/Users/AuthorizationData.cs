using Common.Extensions;
using REST.Security;
using Newtonsoft.Json;
using REST;
using System;
using Common.Security;
using Microsoft.AspNetCore.Identity.MongoDB;
using System.Collections.Generic;
using System.Security.Claims;
using System.Linq;
using Microsoft.AspNetCore.Identity;
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

		public List<IdentityUserLogin> Logins = new List<IdentityUserLogin>();
		public List<IdentityUserClaim> Claims = new List<IdentityUserClaim>();
		public List<IdentityUserToken> Tokens = new List<IdentityUserToken>();
		public List<string> Roles = new List<string>();

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

		public AuthorizationData(string userName, string password)
		{
			// URGENT TODO: we can't really use the password here
			// BECAUSE we need to change the key to the passphrase when the user auth changes
			PasswordHash = HashPassword(password);

			if (!ValidationExtensions.IsStrongPassword(password, out var error))
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

		public AuthorizationData() { }

		/*
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
		*/

		public virtual void AddLogin(UserLoginInfo login)
		{
			Logins.Add(new IdentityUserLogin(login));
		}

		public virtual void RemoveLogin(string loginProvider, string providerKey)
		{
			Logins.RemoveAll(l => l.LoginProvider == loginProvider && l.ProviderKey == providerKey);
		}

		public virtual void AddClaim(Claim claim)
		{
			Claims.Add(new IdentityUserClaim(claim));
		}

		public virtual void RemoveClaim(Claim claim)
		{
			Claims.RemoveAll(c => c.Type == claim.Type && c.Value == claim.Value);
		}

		public virtual void ReplaceClaim(Claim existingClaim, Claim newClaim)
		{
			var claimExists = Claims
				.Any(c => c.Type == existingClaim.Type && c.Value == existingClaim.Value);
			if (!claimExists)
			{
				// note: nothing to update, ignore, no need to throw
				return;
			}
			RemoveClaim(existingClaim);
			AddClaim(newClaim);
		}

		private IdentityUserToken GetToken(string loginProider, string name)
			=> Tokens
				.FirstOrDefault(t => t.LoginProvider == loginProider && t.Name == name);

		public virtual void SetToken(string loginProider, string name, string value)
		{
			var existingToken = GetToken(loginProider, name);
			if (existingToken != null)
			{
				existingToken.Value = value;
				return;
			}

			Tokens.Add(new IdentityUserToken
			{
				LoginProvider = loginProider,
				Name = name,
				Value = value
			});
		}

		public virtual string GetTokenValue(string loginProider, string name)
		{
			return GetToken(loginProider, name)?.Value;
		}

		public virtual void RemoveToken(string loginProvider, string name)
		{
			Tokens.RemoveAll(t => t.LoginProvider == loginProvider && t.Name == name);
		}

		public bool CanVerify()
		{
			return true;
		}
	}
}
