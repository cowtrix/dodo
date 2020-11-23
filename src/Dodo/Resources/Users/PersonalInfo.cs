using Dodo.LocalGroups;
using Dodo.Users.Tokens;
using Dodo.Email;
using Resources;
using Resources.Security;
using System.Collections.Generic;

namespace Dodo.Users
{
	public class EmailPreferences
	{
		[View(EPermissionLevel.OWNER)]
		public bool WeeklyUpdate { get; set; }
		[View(EPermissionLevel.OWNER)]
		public bool DailyUpdate { get; set; }
		[View(EPermissionLevel.OWNER)]
		public bool NewNotifications { get; set; }
	}

	public class PersonalInfo
	{
		[Email]
		[PatchCallback(nameof(OnEmailChange))]
		[View(EPermissionLevel.OWNER)]
		public string Email;
		[View(EPermissionLevel.OWNER)]
		public bool EmailConfirmed { get; set; }
		[View(EPermissionLevel.OWNER)]
		public ResourceReference<LocalGroup> LocalGroup { get; set; }
		[View(EPermissionLevel.OWNER)]
		public string TimezoneID { get; set; }
		[View(EPermissionLevel.USER)]
		[VerifyObject]
		public EmailPreferences EmailPreferences { get; set; } = new EmailPreferences();

		/// <summary>
		/// This happens within a resource lock through the PatchCallback attribute
		/// when we patch the email field
		/// </summary>
		/// <param name="requester"></param>
		/// <param name="passphrase"></param>
		/// <param name="oldValue"></param>
		/// <param name="newValue"></param>
		public void OnEmailChange(object requester, Passphrase passphrase, string oldValue, string newValue)
		{
			if(oldValue == newValue)
			{
				return;
			}
			var user = (User)requester;
			var context = new AccessContext(user, passphrase);
			EmailConfirmed = false;
			user.TokenCollection.RemoveAllOfType<VerifyEmailToken>(context, EPermissionLevel.OWNER, context.User);
			var token = user.TokenCollection.AddOrUpdate(context.User, new VerifyEmailToken()) as VerifyEmailToken;
			EmailUtility.SendEmail(
				new EmailAddress
				{
					Email = context.User.PersonalData.Email,
					Name = context.User.Name
				},
				$"Verify Your Email With {Dodo.DodoApp.PRODUCT_NAME}",
				"Callback",
				new Dictionary<string, string>
				{
					{ "MESSAGE", $"You just changed your {Dodo.DodoApp.PRODUCT_NAME}. To verify your email address, please click the button below." },
					{ "CALLBACK_NAME", "Verify Email Address" },
					{ "CALLBACK_URL", $"{Dodo.DodoApp.NetConfig.FullURI}/{UserService.RootURL}/{UserService.VERIFY_EMAIL}?token={token.Token}" }
				});
			token.ConfirmationEmailRequestCount++;
			user.TokenCollection.AddOrUpdate(user, token);
		}
	}
}
