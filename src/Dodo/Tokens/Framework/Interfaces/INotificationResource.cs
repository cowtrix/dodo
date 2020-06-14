using Resources;
using System.Collections.Generic;

namespace Dodo.Users.Tokens
{
	public interface INotificationResource
	{
		IEnumerable<Notification> GetNotifications(AccessContext context, EPermissionLevel permissionLevel);
	}
}
