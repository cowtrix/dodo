using Common.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Generic;
using System.Security.Authentication;

namespace Resources.Security
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
		public string TokenKey { get; set; }

		public Passphrase(string value, TimeSpan? timeout = null)
		{
			TemporaryTokenManager.SetTemporaryToken(value, out var key, timeout);
			TokenKey = key;
		}

		public string Value
		{
			get
			{
				if(string.IsNullOrEmpty(TokenKey))
				{
					return null;
				}
				if(!TemporaryTokenManager.CheckToken(TokenKey, out var token))
				{
					throw new AuthenticationException("Token Expired or Invalid");
				}
				return token;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Passphrase passphrase &&
				   TokenKey == passphrase.TokenKey;
		}

		public override int GetHashCode()
		{
			var hashCode = 1853675142;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TokenKey);
			return hashCode;
		}
	}
}