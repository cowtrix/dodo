using Dodo.Users;
using Newtonsoft.Json;
using Dodo.Users.Tokens;
using Common;
using Resources;

namespace Dodo
{
	public class EncryptedNotificationToken : Token, INotificationToken
	{
		public override bool Encrypted => true;

		public ResourceReference<User> Creator { get; set; }

		[JsonProperty]
		Notification m_notification;
		public Notification GetNotification(AccessContext context) => m_notification;

		public EncryptedNotificationToken() { }

		public EncryptedNotificationToken(User creator, string source, string message, bool canDelete)
		{
			Creator = new ResourceReference<User>(creator);
			m_notification = new Notification(source, message, canDelete);
		}
	}
}
