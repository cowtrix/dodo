using Dodo.Users;
using Resources.Security;

namespace Dodo
{
	/// <summary>
	/// This is the basic authentication packet that we pass around the program in order to authenticate a user.
	/// It contains a reference to the target user, and their unlocked passphrase which has been released from
	/// their user account by the request and encrypted in the session cookie.
	/// </summary>
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
			hashCode = hashCode * -1521134295 + SecurityExtensions.GenerateID(User, Passphrase, User.Guid.ToString()).GetHashCode();
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
