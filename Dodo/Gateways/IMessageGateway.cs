using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;

namespace XR.Dodo
{
	
	public readonly struct Message
	{
		public readonly User Owner;
		public readonly string Content;
		public readonly DateTime TimeStamp;
		public readonly string UUID;

		public Message(User owner, string content)
		{
			Owner = owner;
			Content = content;
			TimeStamp = DateTime.Now;
			UUID = Guid.NewGuid().ToString();
		}
	}

	public class User
	{
		public string Name;
		public string PhoneNumber;
		public int TelegramUser;
	}

	public interface IMessageGateway
	{
		Func<Message, UserSession, IEnumerable<Message>> ProcessMessage { get; set; }
	}
}
