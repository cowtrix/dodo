using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dodo.Dodo
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

	public class ServerMessage
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

	public class ServerMessageChoice : ServerMessage
	{
		public List<string> Commands;
		public ServerMessageChoice(string content, List<string> commands) : base(content)
		{
			Commands = commands;
		}
	}
}
