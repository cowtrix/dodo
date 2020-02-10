using Common;
using System;

namespace REST
{
	/// <summary>
	/// This exception should be thrown when any kind of security breach is detected
	/// It will try very hard to notify the people it needs to
	/// </summary>
	public partial class SecurityException : Exception
	{
		public delegate void OnBroadcastDelegate(SecurityException exception);
		public OnBroadcastDelegate OnBroadcast;

		public SecurityException()
		{
			BroadCast();
		}

		public SecurityException(string message) : base(message)
		{
			BroadCast();
		}

		public SecurityException(string message, Exception innerException) : base(message, innerException)
		{
			BroadCast();
		}

		void BroadCast()
		{
			try
			{
				OnBroadcast?.Invoke(this);
			}
			catch(Exception e)
			{
				Logger.Exception(e);
			}
		}
	}
}
