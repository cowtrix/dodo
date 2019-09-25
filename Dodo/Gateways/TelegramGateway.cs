﻿using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Common;
using Telegram.Bot.Types.ReplyMarkups;

namespace XR.Dodo
{
	public class TelegramGateway : IMessageGateway
	{
		private class OutgoingMessage
		{
			public ServerMessage Message;
			public UserSession Session;
		}

		private Queue<OutgoingMessage> m_outbox = new Queue<OutgoingMessage>();
		readonly string m_secret;
		TelegramBotClient m_botClient;
		private int m_maxMsgPerSecond = 30;

		public EGatewayType Type { get { return EGatewayType.Telegram; } }

		public string UserName { get; private set; }

		public TelegramGateway(string secret)
		{
			m_secret = secret;
			if (DodoServer.Dummy)
			{
				return;
			}
			var workerThread = new Task(async () =>
			{
				try
				{
					m_botClient = new TelegramBotClient(m_secret);
					var me = m_botClient.GetMeAsync().Result;
					UserName = me.Username;
					Logger.Debug($"Started Telegram bot with ID: {me.Id}, User {me.Username} and Name: {me.FirstName}.");
					m_botClient.OnMessage += Bot_OnMessage;
					m_botClient.StartReceiving();
				}
				catch (Exception e)
				{
					Logger.Exception(e, "Failed to start Telegram Bot");
				}

				var timer = new Stopwatch();

				timer.Start();
				while(true)
				{
					try
					{
						int maxPerSecond = m_maxMsgPerSecond;
						int messagesSent = 0;
						while(messagesSent < maxPerSecond && m_outbox.Count > 0)
						{
							var outmsg = m_outbox.Dequeue();
							await SendMessageAsync(outmsg);
							messagesSent++;
						}
						while(timer.Elapsed < TimeSpan.FromSeconds(1))
						{
							await Task.Delay(100);
						}
						timer.Restart();
					}
					catch (Exception e)
					{
						Logger.Exception(e);
					}
				}
			});
			workerThread.Start();
		}

		public void SendNumberRequest(string msg, UserSession session)
		{
			var rkm = new ReplyKeyboardMarkup();
			rkm.OneTimeKeyboard = true;
			rkm.Keyboard = new[]
			{
				new[]
				{
					new KeyboardButton("Share")
					{
						RequestContact = true
					}
				}
			};
			m_botClient.SendTextMessageAsync(session.GetUser().TelegramUser, 
				msg, replyMarkup:rkm);
		}

		public void SendMessage(ServerMessage message, UserSession session)
		{
			Logger.Debug($"Telegram >> {session.GetUser()}: {message.Content.Substring(0, Math.Min(message.Content.Length, 32))}{(message.Content.Length > 32 ? "..." : "")}");
			if(DodoServer.Dummy)
			{
				session.GetUser().OnMsgReceived?.Invoke(message, session);
				return;
			}
			m_outbox.Enqueue(new OutgoingMessage()
			{
				Message = message,
				Session = session,
			});
		}

		private async Task SendMessageAsync(OutgoingMessage msg)
		{
			if(msg == null || msg.Message == default(ServerMessage))
			{
				Logger.Warning($"Attempted to send a null message to {msg?.Session?.GetUser()}");
				return;
			}
			if (msg.Message.Content.Length > 160)
			{
				Logger.Warning("Message length > sms limit");
			}
			await m_botClient.SendTextMessageAsync(msg.Session.GetUser().TelegramUser, msg.Message.Content);
		}

		public void Broadcast(ServerMessage serverMessage, IEnumerable<User> users)
		{
			foreach(var user in users)
			{
				SendMessage(serverMessage, DodoServer.SessionManager.GetOrCreateSession(user));
			}
		}

		async void Bot_OnMessage(object sender, MessageEventArgs e)
		{
			try
			{
				var message = e.Message.Text;
				var userID = e.Message.From.Id;

				if(userID == e.Message.Contact?.UserId)
				{
					// Verification recieveds
					DodoServer.SessionManager.TryVerify(e.Message.Contact.PhoneNumber, userID);
				}

				var outgoing = GetMessage(message, userID, out var session);
				if(!string.IsNullOrEmpty(outgoing.Content))
				{
					SendMessage(outgoing, session);
				}
			}
			catch (Exception exception)
			{
				Logger.Exception(exception);
			}
		}

		ServerMessage GetMessage(string message, int userID, out UserSession session)
		{
			session = null;
			if (!string.IsNullOrEmpty(message))
			{
				var user = DodoServer.SessionManager.GetOrCreateUserFromTelegramNumber(userID);
				session = DodoServer.SessionManager.GetOrCreateSession(user);
				Logger.Debug($"{session.GetUser()} >> Telegram: {message.Substring(0, Math.Min(message.Length, 32))}{(message.Length > 32 ? "..." : "")}");
				var customMessage = new UserMessage(user, message, this, userID.ToString());
				return session.ProcessMessage(customMessage, session);
			}
			return default(ServerMessage);
		}

		public ServerMessage FakeMessage(string message, int userID)
		{
			return GetMessage(message, userID, out var _);
		}
	}
}
