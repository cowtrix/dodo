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
		private string m_value { get; set; }

		public Passphrase(string value, TimeSpan? timeout = null)
		{
			//TemporaryTokenManager.SetTemporaryToken(value, out var key, timeout);
			m_value = value;
		}

		public string Value
		{
			get
			{
				return m_value;
			}
		}

		public override bool Equals(object obj)
		{
			return obj is Passphrase passphrase &&
				   Value == passphrase.Value;
		}

		public override int GetHashCode()
		{
			var hashCode = 1853675142;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Value);
			return hashCode;
		}
	}
}
