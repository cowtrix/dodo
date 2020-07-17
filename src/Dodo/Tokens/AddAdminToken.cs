using Common.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo.Users.Tokens
{
	public class UserAddedAsAdminToken : AutoExecutableToken, INotificationToken
	{
		[JsonProperty(TypeNameHandling = TypeNameHandling.None)]
		public ResourceReference<IAdministratedResource> Resource { get; private set; }
		[JsonProperty]
		public byte[] Token { get; private set; }
		[JsonIgnore]
		public override bool Encrypted => true;
		[JsonProperty]
		private Notification m_notification;

		public UserAddedAsAdminToken() { }

		public UserAddedAsAdminToken(IAdministratedResource resource, Passphrase temporaryPassword, string publicKey) : base()
		{
			Resource = resource.CreateRef();
			Token = AsymmetricSecurity.Encrypt(temporaryPassword.Value, publicKey);
			m_notification = new Notification(Guid, resource.Name, $"You have been added as an Administrator to {Resource.Name}", null, ENotificationType.Alert, GetVisibility());
		}

		protected override bool OnExecuted(AccessContext context)
		{
			var privateKey = context.User.AuthData.PrivateKey.GetValue(context.Passphrase);
			var tempPass = new Passphrase(AsymmetricSecurity.Decrypt<string>(Token, privateKey));
			using (var rscLocker = new ResourceLock(Resource.GetValue()))
			{
				var resource = rscLocker.Value as IAdministratedResource;
				// Change the admin access from temp us
				resource.CompleteAdminInvite(context, tempPass);
				ResourceUtility.GetManagerForResource(resource).Update(resource, rscLocker);
			}
			return true;
		}

		public Notification GetNotification(AccessContext context)
		{
			return m_notification;
		}

		public override EPermissionLevel GetVisibility() => EPermissionLevel.OWNER;
	}
}
