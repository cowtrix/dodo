using Common.Security;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo.Users.Tokens
{
	public class UserAddedAsAdminToken : AutoExecutableToken, INotificationToken, IRemovableToken, IMyRebellionToken
	{
		[JsonProperty(TypeNameHandling = TypeNameHandling.None)]
		public ResourceReference<IAdministratedResource> Resource { get; private set; }
		[JsonProperty]
		public byte[] Token { get; private set; }
		[JsonIgnore]
		public override bool Encrypted => true;
		[JsonIgnore]
		public override bool ShouldRemove => false;
		[JsonProperty]
		private Notification m_notification;

		public UserAddedAsAdminToken() { }

		public UserAddedAsAdminToken(ResourceReference<IAdministratedResource> resource) : base()
		{
			Resource = resource;
			m_notification = new Notification(Guid, resource.Name, $"You have been added as an Administrator to {Resource.Name}", null, ENotificationType.Alert, GetVisibility());
		}

		public UserAddedAsAdminToken(IAdministratedResource resource) : this(resource.CreateRef()) { }

		public UserAddedAsAdminToken(IAdministratedResource resource, Passphrase temporaryPassword, string publicKey) : base()
		{
			Resource = resource.CreateRef();
			Token = AsymmetricSecurity.Encrypt(temporaryPassword.Value, publicKey);
			m_notification = new Notification(Guid, resource.Name, $"You have been added as an Administrator to {Resource.Name}", null, ENotificationType.Alert, GetVisibility());
		}

		protected override bool OnExecuted(AccessContext context)
		{
			if (Token == null)
			{
				// Probably the user was added as admin on resource creation
				return true;
			}
			var privateKey = context.User.AuthData.PrivateKey.GetValue(context.Passphrase);
			var tempPass = new Passphrase(AsymmetricSecurity.Decrypt<string>(Token, privateKey));
			using (var rscLocker = new ResourceLock(Resource.GetValue()))
			{
				var resource = rscLocker.Value as IAdministratedResource;
				// Change the admin access from temp us
				resource.CompleteAdminInvite(context, tempPass);
				ResourceUtility.GetManagerForResource(resource).Update(resource, rscLocker);
			}
			using (var rscLocker = new ResourceLock(context.User))
			{
				// Reset token so the new user can decrypt it
				var user = rscLocker.Value as User;
				user.TokenCollection.AddOrUpdate(user, new UserAddedAsAdminToken(Resource));
				ResourceUtility.GetManager<User>().Update(user, rscLocker);
			}
			return true;
		}

		public Notification GetNotification(AccessContext context)
		{
			return m_notification;
		}

		public override EPermissionLevel GetVisibility() => EPermissionLevel.OWNER;

		[JsonIgnore]
		public IResourceReference Reference => Resource;
	}
}
