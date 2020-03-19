using Common.Extensions;
using Resources.Security;
using Newtonsoft.Json;
using System;
using Common.Security;

namespace Dodo.Users.Tokens
{
	[SingletonToken]
	public class VerifyEmailToken : RedeemableToken, INotificationToken
	{
		const int TOKEN_SIZE = 64;

		[JsonProperty]
		public string Token { get; private set; }

		public override void OnAdd(User parent)
		{
			Token = KeyGenerator.GetUniqueKey(TOKEN_SIZE);
		}

		public string GetNotification(AccessContext context) => "You should check your email and verify your email address with us.";
	}
}
