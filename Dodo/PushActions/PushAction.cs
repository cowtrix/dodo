using Common;
using Common.Security;
using Newtonsoft.Json;

namespace Dodo.Users
{
	public abstract class PushAction
	{
		public abstract string Message { get; }
		public abstract bool AutoFire { get; }
		[JsonProperty]
		public bool HasExecuted { get; private set; }

		protected virtual void ExecuteInternal(User user, Passphrase passphrase) { }
		public void Execute(User user, Passphrase passphrase)
		{
			if(HasExecuted)
			{
				return;
			}
			ExecuteInternal(user, passphrase);
			HasExecuted = true;
		}
	}
}
