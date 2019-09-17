﻿using System;

namespace XR.Dodo
{
	public enum EGatewayType
	{
		SMS,
		Telegram,
	}

	public readonly struct UserMessage
	{
		public readonly string OwnerID;
		public readonly string Content;
		public readonly DateTime TimeStamp;
		public readonly string MessageID;
		public readonly string Source;
		public readonly EGatewayType GatewayType;

		public UserSession GetSession()
		{
			return DodoServer.SessionManager.GetSessionFromUserID(OwnerID);
		}

		public UserMessage(User owner, string content, EGatewayType gatewayType, string source)
		{
			OwnerID = owner.UUID;
			Content = content;
			TimeStamp = DateTime.Now;
			MessageID = Guid.NewGuid().ToString();
			GatewayType = gatewayType;
			Source = source;
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