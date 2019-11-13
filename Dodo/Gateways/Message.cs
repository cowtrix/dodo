using System;
using System.Collections.Generic;

namespace Dodo
{
	public abstract class Message
	{
		public Guid GUID;
		public DateTime TimeStamp;
		public string Content;

		public Message() { }

		public Message(string content)
		{
			GUID = Guid.NewGuid();
			Content = content;
			TimeStamp = DateTime.Now;
		}
	}

	public abstract class ClientMessage : Message
	{
		public ClientMessage(string content) : base(content) { }
	}

	public abstract class ServerMessage : Message
	{
		public ServerMessage(string content) : base(content) { }
	}

	public class ServerMessageMultipleChoice : Message
	{
		public List<string> Choices;
		public ServerMessageMultipleChoice(string content, IEnumerable<string> options) : base(content)
		{
			Choices = new List<string>(options);
		}
	}
}
