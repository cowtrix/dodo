using System;
using Common.Extensions;
using Resources;

namespace Dodo
{
	public enum ENotificationType
	{
		Alert,
		Calendar,
		Announcement,

		Twitter = 200,
		Telegram = 201,
	}

	public class Notification
	{
		public string Type { get; set; }
		public DateTime Timestamp { get; set; }
		public string Source { get; set; }
		public string Message { get; set; }
		public string Link { get; set; }
		public Guid Guid { get; set; }
		public bool CanDelete { get; set; }
		public EPermissionLevel PermissionLevel { get; set; }

		public Notification(Guid tokenGuid, string source, string message, string link, ENotificationType type, EPermissionLevel permissionLevel, bool canDelete = false)
		{
			Guid = tokenGuid;
			Source = source;
			Message = StringExtensions.TextToHtml(message);
			Timestamp = DateTime.UtcNow;
			CanDelete = canDelete;
			Link = link;
			Type = type.ToString().ToCamelCase();
			PermissionLevel = permissionLevel;
		}
	}
}
