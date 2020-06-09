using Common.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Generic;
using System.Security.Authentication;

namespace Resources.Security
{
	public struct Passphrase
	{
		private string m_value { get; set; }

		public Passphrase(string value, TimeSpan? timeout = null)
		{
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
