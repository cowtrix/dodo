using Common;
using System;
using System.Collections;
using System.Diagnostics;

namespace Resources.Security
{
	public class SecurityEvent
	{
		public StackTrace StackTrace { get; set; }
		public Exception Exception { get; set; }
		public string Message { get; set; }
		public DateTime TimeStampUTC { get; set; }
	}

	public static class SecurityWatcher	
	{
		public delegate void OnBroadcastDelegate(SecurityEvent exception);
		public static OnBroadcastDelegate OnBroadcast { get; set; }
		private static PersistentStore<Guid, SecurityEvent> m_events = new PersistentStore<Guid, SecurityEvent>("events", "sec");

		public static Guid RegisterEvent(Exception exception, string message = null)
		{
			var ev = new SecurityEvent
			{
				Exception = exception,
				StackTrace = new StackTrace(),
				Message = message,
				TimeStampUTC = DateTime.UtcNow,
			};
			Logger.Exception(exception);
			var guid = Guid.NewGuid();
			m_events[guid] = ev;
			BroadCast(ev);
			return guid;
		}

		public static Guid RegisterEvent(string message)
		{
			var ev = new SecurityEvent
			{
				StackTrace = new System.Diagnostics.StackTrace(),
				Message = message,
				TimeStampUTC = DateTime.UtcNow,
			};
			Logger.Error($"SECURITY EVENT LOGGED: {message}");
			var guid = Guid.NewGuid();
			m_events[guid] = ev;
			BroadCast(ev);
			return guid;
		}

		private static void BroadCast(SecurityEvent ev)
		{
			try
			{
				OnBroadcast?.Invoke(ev);
			}
			catch(Exception e)
			{
				Logger.Exception(e);
			}
		}
	}
}
