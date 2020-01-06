using Common.Extensions;
using Common.Security;
using Newtonsoft.Json;
using System;

namespace Dodo.Users
{
	[SingletonPushAction]
	public class VerifyEmailAction : PushAction
	{
		[JsonProperty]
		public string Token { get; private set; }

		public VerifyEmailAction()
		{
			Token = StringExtensions.RandomString(64);
#if DEBUG
			Console.WriteLine(Token);
#endif
		}

		public override string GetNotificationMessage()
		{
			return "You should check your email and verify your email address with us.";
		}
	}
}
