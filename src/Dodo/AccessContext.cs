using Dodo.Users;
using REST.Security;

namespace Dodo
{
	public readonly struct AccessContext
	{
		public readonly User User;
		public readonly Passphrase Passphrase;

		public AccessContext(User user, Passphrase passphrase)
		{
			User = user;
			Passphrase = passphrase;
		}
	}
}
