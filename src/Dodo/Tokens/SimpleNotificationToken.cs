using Newtonsoft.Json;
using Dodo.Users.Tokens;
using Resources;
using Dodo.Users;

namespace Dodo
{
	public class SimpleNotificationToken : Token, INotificationToken, IRemovableToken
	{
		public override bool Encrypted => false;

		public ResourceReference<User> Creator { get; set; }

		public bool ShouldRemove { get; set; }

		public EPermissionLevel PermissionLevel { get; set; }

		public override EPermissionLevel GetVisibility() => PermissionLevel;

		[JsonProperty]
		Notification m_notification;
		public Notification GetNotification(AccessContext context) => m_notification;

		public void OnRemove(AccessContext parent)
		{
		}

		public SimpleNotificationToken() { }

		public SimpleNotificationToken(User creator, string source, string message, string link, ENotificationType type, EPermissionLevel permissionLevel, bool canDelete) : base()
		{
			Creator = creator.CreateRef();
			PermissionLevel = permissionLevel;
			m_notification = new Notification(Guid, source, message, link, type, permissionLevel, canDelete);
		}
	}
}
