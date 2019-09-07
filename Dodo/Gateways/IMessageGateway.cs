using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;

namespace XR.Dodo
{
	
	public readonly struct Message
	{
		public readonly string OwnerID;
		public readonly string Content;
		public readonly DateTime TimeStamp;
		public readonly string MessageID;

		public UserSession GetSession()
		{
			return DodoServer.SessionManager.GetSessionFromUUID(OwnerID);
		}

		public Message(User owner, string content)
		{
			OwnerID = owner.UUID;
			Content = content;
			TimeStamp = DateTime.Now;
			MessageID = Guid.NewGuid().ToString();
		}
	}

	public class User
	{
		public string Name;
		public string PhoneNumber;
		public int TelegramUser;
		public string UUID;

		public User()
		{
			UUID = Guid.NewGuid().ToString();
		}
	}

	public interface IMessageGateway
	{
		Func<Message, UserSession, IEnumerable<Message>> ProcessMessage { get; set; }
	}
}
