using Dodo.Users;
using Newtonsoft.Json;
using Dodo.Users.Tokens;
using Resources;

namespace Dodo
{
	public class EncryptedNotificationToken : Token, INotificationToken
	{
		public override bool Encrypted => true;

		public ResourceReference<User> Creator { get; set; }

		public EPermissionLevel PermissionLevel { get; set; }

		[JsonProperty]
		Notification m_notification;
		public Notification GetNotification(AccessContext context) => m_notification;

		public override EPermissionLevel GetVisibility() => PermissionLevel;

		public EncryptedNotificationToken() { }

		public EncryptedNotificationToken(User creator, string source, string message, string link, ENotificationType type, EPermissionLevel permissionLevel, bool canDelete)
		{
			Creator = creator.CreateRef();
			PermissionLevel = permissionLevel;
			m_notification = new Notification(Guid, source, message, link, type, permissionLevel, canDelete);
		}
	}
}
