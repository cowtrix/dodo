using Common.Security;
using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo.Users
{
	public class AddAdminAction : ExecutableToken
	{
		[JsonProperty]
		public ResourceReference<GroupResource> Resource { get; private set; }
		[JsonProperty]
		public byte[] Token { get; private set; }
		public override bool CanRemove => true;

		public AddAdminAction(GroupResource resource, Passphrase temporaryPassword, string publicKey) : base()
		{
			Resource = new ResourceReference<GroupResource>(resource);
			Token = AsymmetricSecurity.Encrypt(temporaryPassword.Value, publicKey);
		}

		protected override void ExecuteInternal(AccessContext context)
		{
			Resource.CheckValue();
			var privateKey = context.User.AuthData.PrivateKey.GetValue(context.Passphrase);
			var tempPass = new Passphrase(AsymmetricSecurity.Decrypt<string>(Token, privateKey));
			using (var rscLocker = new ResourceLock(Resource.GetValue()))
			{
				var resource = rscLocker.Value as GroupResource;
				// Change the admin access from temp us
				resource.AddOrUpdateAdmin(new AccessContext(context.User, tempPass), context.User, context.Passphrase);
				ResourceUtility.GetManagerForResource(resource).Update(resource, rscLocker);
			}
			Removed = true;
		}

		public override string GetNotificationMessage()
		{
			return $"You have been added as an Administrator to {Resource.GetValue().Name}";
		}
	}
}
