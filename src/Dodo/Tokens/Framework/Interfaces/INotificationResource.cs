using Resources;
using System.Collections.Generic;

namespace Dodo.Users.Tokens
{
	public interface INotificationResource : ITokenResource
	{
		IEnumerable<Notification> GetNotifications(AccessContext context, EPermissionLevel permissionLevel);
	}
}
