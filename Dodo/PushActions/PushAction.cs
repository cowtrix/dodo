using Common;
using Common.Extensions;
using Common.Security;
using Newtonsoft.Json;

namespace Dodo.Users
{
	public abstract class PushAction
	{
		public abstract string Message { get; }
		public abstract bool AutoFire { get; }

		public abstract void Execute(User user, Passphrase passphrase);
	}

	public class VerifyEmailAction : PushAction
	{
		public override string Message => "";

		public override bool AutoFire => false;

		[JsonProperty]
		public string Token { get; private set; }

		public VerifyEmailAction(User user)
		{
			Token = StringExtensions.RandomString(64);
		}

		public override void Execute(User user, Passphrase passphrase)
		{
		}
	}
}
