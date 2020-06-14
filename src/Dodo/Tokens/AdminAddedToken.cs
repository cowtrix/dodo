using Dodo.Users;
using Newtonsoft.Json;
using Dodo.Users.Tokens;
using Common;

namespace Dodo
{
	public class EncryptedNotificationToken : Token, INotificationToken
	{
		public override bool Encrypted => true;
		[JsonProperty]
		Notification m_notification;
		public Notification GetNotification(AccessContext context) => m_notification;

		public EncryptedNotificationToken(string source, string message, bool canDelete)
		{
			m_notification = new Notification(source, message, canDelete);
		}
	}

	public class SimpleNotificationToken : Token, INotificationToken
	{
		public override bool Encrypted => false;
		[JsonProperty]
		Notification m_notification;
		public Notification GetNotification(AccessContext context) => m_notification;

		public SimpleNotificationToken(string source, string message, bool canDelete)
		{
			m_notification = new Notification(source, message, canDelete);
		}
	}
}
