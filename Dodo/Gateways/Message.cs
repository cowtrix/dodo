using Newtonsoft.Json;
using System;
using System.Collections.Generic;
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

		[JsonIgnore]
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

		public override bool Equals(object obj)
		{
			return obj is ServerMessage message &&
				   Content == message.Content &&
				   TimeStamp == message.TimeStamp &&
				   MessageID == message.MessageID;
		}

		public override int GetHashCode()
		{
			var hashCode = 1031664614;
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(Content);
			hashCode = hashCode * -1521134295 + TimeStamp.GetHashCode();
			hashCode = hashCode * -1521134295 + EqualityComparer<string>.Default.GetHashCode(MessageID);
			return hashCode;
		}

		public static bool operator ==(ServerMessage left, ServerMessage right)
		{
			return left.Equals(right);
		}

		public static bool operator !=(ServerMessage left, ServerMessage right)
		{
			return !(left == right);
		}
	}
}
