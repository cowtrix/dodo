using Resources;
using System;
using System.Collections.Generic;

namespace Dodo.Users.Tokens
{
	public interface INotificationResource : ITokenResource
	{
		public const string ACTION_NOTIFICATION = "notifications";
		IEnumerable<Notification> GetNotifications(AccessContext context, EPermissionLevel permissionLevel);
		bool DeleteNotification(AccessContext context, EPermissionLevel permissionLevel, Guid notification);
	}
}
