using Common.Security;
using Dodo.Users;
using Resources.Security;
using System;
using System.Collections.Generic;

namespace Dodo
{
	public readonly struct AccessContext
	{
		public readonly User User;
		public readonly Passphrase Passphrase;

		public AccessContext(User user, string passphrase)
		{
			User = user;
			Passphrase = new Passphrase(passphrase);
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

		/// <summary>
		/// Is the state of the AccessContext valid? Note that this doesn't tell
		/// you if there is an authenticated user or not.
		/// </summary>
		/// <returns>True if the context is valid, false if it is not</returns>
		public bool Challenge()
		{
			return User == null || PasswordHasher.VerifyHashedPassword(User.AuthData.PassphraseHash, Passphrase.Value);
		}
	}
}
