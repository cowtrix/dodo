using Common.Extensions;
using Common.Security;
using Newtonsoft.Json;

namespace Dodo.Users
{
	public class VerifyEmailAction : PushAction
	{
		public override string Message => "You should check your email and verify your email address with us.";

		public override bool AutoFire => false;

		[JsonProperty]
		public string Token { get; private set; }

		public VerifyEmailAction(User user)
		{
			Token = StringExtensions.RandomString(64);
		}
	}
}
