using Common.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using REST;
using System;
using System.Collections.Generic;
using System.Security.Authentication;

namespace REST.Security
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
		[JsonProperty]
		[BsonElement]
		public string TokenKey { get; private set; }
		[JsonProperty]
		[BsonElement]
		public string Data { get; private set; }

		public Passphrase(string value, TimeSpan? timeout = null)
		{
			TemporaryTokenManager.GetTemporaryToken(out var tokenKey, out var token, timeout);
			TokenKey = tokenKey;
			Data = SymmetricSecurity.Encrypt(value, token);
		}

		internal Passphrase(string tokenKey, string data)
		{
			TokenKey = tokenKey;
			Data = data;
		}

		public string Value
		{
			get
			{
				if(string.IsNullOrEmpty(TokenKey))
				{
					return null;
				}
				if(!TemporaryTokenManager.IsValidToken(TokenKey, out var token))
				{
					throw new AuthenticationException("Token Expired or Invalid");
				}
				return SymmetricSecurity.Decrypt<string>(Data, token);
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Passphrase passphrase &&
				   TokenKey == passphrase.TokenKey &&
				   Data == passphrase.Data;
		}

		public override int GetHashCode()
		{
			var hashCode = 1853675142;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(TokenKey);
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Data);
			return hashCode;
		}
	}
}