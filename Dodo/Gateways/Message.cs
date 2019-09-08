using System;

namespace XR.Dodo
{
	public readonly struct UserMessage
	{
		public readonly string OwnerID;
		public readonly string Content;
		public readonly DateTime TimeStamp;
		public readonly string MessageID;

		public UserSession GetSession()
		{
			return DodoServer.SessionManager.GetSessionFromUserID(OwnerID);
		}

		public UserMessage(User owner, string content)
		{
			OwnerID = owner.UUID;
			Content = content;
			TimeStamp = DateTime.Now;
			MessageID = Guid.NewGuid().ToString();
		}
	}

	public readonly struct ServerMessage
	{
		public readonly string Content;
		public readonly DateTime TimeStamp;
		public readonly string MessageID;

		public ServerMessage(string content)
		{
			Content = content;
			TimeStamp = DateTime.Now;
			MessageID = Guid.NewGuid().ToString();
		}
	}
}
