using System;
using REST;

namespace Dodo.Users
{
	[ViewClass]
	public struct Notification
	{
		public string Message;
		public Guid GUID;
	}
}
