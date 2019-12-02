using Common;
using Common.Security;

namespace Dodo.Users
{
	public abstract class PushAction
	{
		public abstract string Message { get; }
		public abstract void Execute(User user, Passphrase passphrase);
	}
}
