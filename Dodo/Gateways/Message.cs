﻿using System;
using System.Linq;

namespace XR.Dodo
{
	public struct UserMessage
	{
		public string OwnerID;
		public string Content;
		public string[] ContentUpper;
		public DateTime TimeStamp;
		public string MessageID;
		public string Source;
		public IMessageGateway Gateway;

		public UserSession GetSession()
		{
			return DodoServer.SessionManager.GetSessionFromUserID(OwnerID);
		}

		public UserMessage(User owner, string content, IMessageGateway gateway, string source)
		{
			OwnerID = owner.UUID;
			Content = content;
			ContentUpper = content.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries)
				.Select(x => x.ToUpperInvariant()).ToArray();
			TimeStamp = DateTime.Now;
			MessageID = Guid.NewGuid().ToString();
			Gateway = gateway;
			Source = source;
		}
	}

	public struct ServerMessage
	{
		public string Content;
		public DateTime TimeStamp;
		public string MessageID;

		public ServerMessage(string content)
		{
			Content = content;
			TimeStamp = DateTime.Now;
			MessageID = Guid.NewGuid().ToString();
		}
	}
}
