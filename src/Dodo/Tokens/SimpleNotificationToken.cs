using Newtonsoft.Json;
using Dodo.Users.Tokens;
using Resources;
using Dodo.Users;

namespace Dodo
{
	public class SimpleNotificationToken : Token, INotificationToken
	{
		public override bool Encrypted => false;

		public ResourceReference<User> Creator { get; set; }

		[JsonProperty]
		Notification m_notification;
		public Notification GetNotification(AccessContext context) => m_notification;

		public SimpleNotificationToken() { }

		public SimpleNotificationToken(User creator, string source, string message, bool canDelete)
		{
			Creator = new ResourceReference<User>(creator);
			m_notification = new Notification(source, message, canDelete);
		}
	}
}
