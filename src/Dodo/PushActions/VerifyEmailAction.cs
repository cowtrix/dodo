using Common.Extensions;
using Resources.Security;
using Newtonsoft.Json;
using System;
using Common.Security;

namespace Dodo.Users
{
	[SingletonPushAction]
	public class VerifyEmailAction : PushAction
	{
		const int TOKEN_SIZE = 64;

		[JsonProperty]
		public string Token { get; private set; }

		public override void OnAdd()
		{
			Token = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
		}

		public override string GetNotificationMessage()
		{
			return "You should check your email and verify your email address with us.";
		}
	}
}
