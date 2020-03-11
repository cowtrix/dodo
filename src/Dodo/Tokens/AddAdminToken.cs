using Common.Security;
using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo.Users.Tokens
{
	public class AddAdminToken : RedeemableToken, INotificationToken
	{
		[JsonProperty]
		[NotNulResource]
		public ResourceReference<GroupResource> Resource { get; private set; }
		[JsonProperty]
		public byte[] Token { get; private set; }

		public AddAdminToken(GroupResource resource, Passphrase temporaryPassword, string publicKey) : base()
		{
			Resource = new ResourceReference<GroupResource>(resource);
			Token = AsymmetricSecurity.Encrypt(temporaryPassword.Value, publicKey);
		}

		protected override bool OnRedeemed(AccessContext context)
		{
			var privateKey = context.User.AuthData.PrivateKey.GetValue(context.Passphrase);
			var tempPass = new Passphrase(AsymmetricSecurity.Decrypt<string>(Token, privateKey));
			using (var rscLocker = new ResourceLock(Resource.GetValue()))
			{
				var resource = rscLocker.Value as GroupResource;
				// Change the admin access from temp us
				resource.AddOrUpdateAdmin(new AccessContext(context.User, tempPass), context.User, context.Passphrase);
				ResourceUtility.GetManagerForResource(resource).Update(resource, rscLocker);
			}
			return true;
		}

		public string GetNotification(AccessContext context)
		{
			return $"You have been added as an Administrator to {Resource.GetValue().Name}";
		}
	}
}
