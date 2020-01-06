using Newtonsoft.Json;
using SimpleHttpServer.REST;
using Common.Security;

namespace Dodo.Users
{
	public class AddAdminAction : PushAction
	{
		[JsonProperty]
		public ResourceReference<GroupResource> Resource { get; private set; }
		[JsonProperty]
		public byte[] Token { get; private set; }
		public override bool AutoFire => true;
		public override bool CanRemove => true;

		public AddAdminAction(GroupResource resource, Passphrase temporaryPassword, string publicKey) : base()
		{
			Resource = new ResourceReference<GroupResource>(resource);
			Token = AsymmetricSecurity.Encrypt(temporaryPassword.Value, publicKey);
		}

		protected override void ExecuteInternal(User user, Passphrase passphrase)
		{
			Resource.CheckValue();
			var privateKey = user.WebAuth.PrivateKey.GetValue(passphrase);
			var tempPass = new Passphrase(AsymmetricSecurity.Decrypt<string>(Token, privateKey));
			using (var rscLocker = new ResourceLock(Resource.Guid))
			{
				var resource = rscLocker.Value as GroupResource;
				resource.AddAdmin(user, tempPass, user, passphrase);
				ResourceUtility.GetManagerForResource(resource).Update(resource, rscLocker);
			}
		}

		public override string GetNotificationMessage()
		{
			return $"You have been added as an Administrator to {Resource.Value.Name}";
		}
	}
}
