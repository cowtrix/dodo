using System;
using Resources;

namespace Dodo.Users
{
	public class Notification
	{
		public DateTime Timestamp { get; set; }
		public string Source { get; set; }
		public string Message { get; set; }
		public Guid Guid { get; set; }
		public bool Seen { get; set; }
		public bool CanDelete { get; set; }

		public Notification(string source, string message, bool canDelete = false)
		{
			Source = source;
			Message = message;
			Guid = Guid.NewGuid();
			Timestamp = DateTime.UtcNow;
			CanDelete = canDelete;
		}
	}
}
