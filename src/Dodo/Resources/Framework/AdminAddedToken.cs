using Dodo.Users;
using Newtonsoft.Json;
using Dodo.Users.Tokens;

namespace Dodo
{
	public class GroupAdminAddedToken : Token, INotificationToken
	{
		public override bool Encrypted => true;
		[JsonProperty]
		Notification m_notification;
		public Notification GetNotification(AccessContext context) => m_notification;

		public GroupAdminAddedToken(User existingAdmin, User newAdmin)
		{
			m_notification = new Notification("", $"Administrator @{existingAdmin.AuthData.Username} added new Administrator @{newAdmin.AuthData.Username}");
		}
	}
}
