using Common.Extensions;
using Common.Security;
using Dodo.Email;
using Newtonsoft.Json;
using Resources;
using Resources.Security;
using System.Collections.Generic;

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

		public UserAddedAsAdminToken(ResourceReference<IAdministratedResource> resource, User newAdmin)
		{
			Resource = resource;
			if (m_notification == null)
			{
				var subj = $"You have been added as an Administrator to {Resource.Name}";
				var url = $"{Dodo.DodoApp.NetConfig.FullURI}/{resource.Type}/{resource.Slug}";
				m_notification = new Notification(Guid, resource.Name, subj, url, ENotificationType.Alert, GetVisibility());

				var txt = $"You're receiving this email because you are registered with an account at {Dodo.DodoApp.NetConfig.FullURI}\n\n" +
					$"You have been added as an administrator of the {resource.Type} \"{Resource.Name}\". You can view this [here.]({url}) " +
					$"You can view the Adminstrator Panel [here.]({Dodo.DodoApp.NetConfig.FullURI}/edit/{resource.Type}/{resource.Slug})";

				EmailUtility.SendEmail(
					new EmailAddress { Email = newAdmin.PersonalData.Email, Name = newAdmin.Name },
					$"[{Dodo.DodoApp.PRODUCT_NAME}] {subj}",
					"Notification",
					new Dictionary<string, string> { { "MESSAGE", txt } });
			}
		}

		public UserAddedAsAdminToken(IAdministratedResource resource, User newAdmin) : this(resource.CreateRef(), newAdmin) { }

		public UserAddedAsAdminToken(ResourceReference<IAdministratedResource> resource, Passphrase temporaryPassword, string publicKey, User newAdmin) : this(resource, newAdmin)
		{
			Token = AsymmetricSecurity.Encrypt(temporaryPassword.Value, publicKey);
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
				user.TokenCollection.AddOrUpdate(user, new UserAddedAsAdminToken(Resource, user));
				ResourceUtility.GetManager<User>().Update(user, rscLocker);
			}
			return true;
		}

		public Notification GetNotification(AccessContext context)
		{
			return m_notification;
		}

		public override EPermissionLevel GetVisibility() => EPermissionLevel.ADMIN;

		[JsonIgnore]
		public IResourceReference Reference => Resource;
	}
}
