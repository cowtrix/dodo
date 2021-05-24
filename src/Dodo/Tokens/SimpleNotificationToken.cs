using Newtonsoft.Json;
using Dodo.Users.Tokens;
using Resources;
using Dodo.Users;
using Dodo.Email;

namespace Dodo
{
	public class SimpleNotificationToken : Token, INotificationToken, IRemovableToken
	{
		public override bool Encrypted => false;

		public HashedResourceReference Creator { get; set; }

		public bool ShouldRemove { get; set; }

		public EPermissionLevel PermissionLevel { get; set; }

		public override EPermissionLevel GetVisibility() => PermissionLevel;

		[JsonProperty]
		Notification m_notification;
		public Notification GetNotification(AccessContext context) => m_notification;

		public void OnRemove(AccessContext parent)
		{
		}

		public override void OnAdd(ITokenResource parent)
		{
			base.OnAdd(parent);
			if(parent is IPublicResource pub && m_notification.PermissionLevel < EPermissionLevel.ADMIN)
			{
				UserEmailManager.RegisterUpdate(pub, "New Announcement:", m_notification.RawMessage);
			}
		}

		public SimpleNotificationToken() { }

		public SimpleNotificationToken(User creator, string source, string message, string link, ENotificationType type, EPermissionLevel permissionLevel, bool canDelete) : base()
		{
			Creator = new HashedResourceReference(creator.Guid, message);
			PermissionLevel = permissionLevel;
			m_notification = new Notification(Guid, source, message, link, type, permissionLevel, canDelete);
		}
	}
}
