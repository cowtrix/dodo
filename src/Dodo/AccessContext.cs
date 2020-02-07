using Common.Security;
using Dodo.Users;
using REST.Security;
using System;
using System.Collections.Generic;

namespace Dodo
{
	public readonly struct AccessContext
	{
		public readonly User User;
		public readonly Passphrase Passphrase;

		public AccessContext(User user, string password)
		{
			User = user;
			Passphrase = new Passphrase(User.AuthData.PassPhrase.GetValue(new Passphrase(password)));
		}

		public AccessContext(User user, Passphrase passphrase)
		{
			User = user;
			Passphrase = passphrase;
		}

		public override bool Equals(object obj)
		{
			return obj is AccessContext context &&
				   User.GUID.Equals(context.User.GUID) &&
				   Passphrase.Equals(context.Passphrase);
		}

		public override int GetHashCode()
		{
			var hashCode = 2085623975;
			hashCode = hashCode * -1521134295 + SecurityExtensions.GenerateID(User, Passphrase).GetHashCode();
			return hashCode;
		}

		public bool Challenge()
		{
			return User != null && User.AuthData.PassphraseHash == SHA256Utility.SHA256(Passphrase.Value);
		}
	}
}
