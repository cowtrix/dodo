using Common.Security;
using Dodo.Users;
using Resources.Security;
using System;
using System.Collections.Generic;
using System.Text.Json.Serialization;

namespace Dodo
{
	public readonly struct AccessContext
	{
		public readonly User User;
		public readonly Passphrase Passphrase;
		public readonly string UserToken;

		public AccessContext(User user, string passphrase, string userToken = null)
		{
			User = user;
			Passphrase = new Passphrase(passphrase);
			UserToken = userToken;
		}

		public AccessContext(User user, Passphrase passphrase, string userToken = null)
		{
			User = user;
			Passphrase = passphrase;
			UserToken = userToken;
		}

		public override bool Equals(object obj)
		{
			return obj is AccessContext context &&
				   User.Guid.Equals(context.User.Guid) &&
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
