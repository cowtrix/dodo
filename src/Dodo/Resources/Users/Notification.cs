using System;
using Resources;

namespace Dodo
{
	public class Notification
	{
		public DateTime Timestamp { get; set; }
		public string Source { get; set; }
		public string Message { get; set; }
		public Guid Guid { get; set; }
		public bool CanDelete { get; set; }

		public Notification(Guid tokenGuid, string source, string message, bool canDelete = false)
		{
			Guid = tokenGuid;
			Source = source;
			Message = message;
			Timestamp = DateTime.UtcNow;
			CanDelete = canDelete;
		}
	}
}
