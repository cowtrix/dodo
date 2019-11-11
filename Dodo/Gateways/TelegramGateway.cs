using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;
using Common;
using Telegram.Bot.Types.ReplyMarkups;
using System.Linq;
using Dodo.Users;

namespace Dodo.Gateways
{
	/*public class TelegramGateway : IMessageGateway
	{
		private class OutgoingMessage
		{
			public ServerMessage Message;
			public User Target;
		}

		private Queue<OutgoingMessage> m_outbox = new Queue<OutgoingMessage>();
		readonly string m_secret;
		TelegramBotClient m_botClient;
		private int m_maxMsgPerSecond = 30;

		public string BotUsername { get; private set; }

		public TelegramGateway(RebellionBotConfiguration.TelegramConfiguration config)
		{
			m_secret = config.BotSecret;
			var workerThread = new Task(async () =>
			{
				try
				{
					m_botClient = new TelegramBotClient(m_secret);
					var me = m_botClient.GetMeAsync().Result;
					BotUsername = me.Username;
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

		public void SendNumberRequest(string msg, User target)
		{
			var rkm = new ReplyKeyboardMarkup();
			rkm.OneTimeKeyboard = true;

			rkm.Keyboard = new[]
			{
				new[]
				{
					new KeyboardButton("Share Your Number\nPRESS HERE TO CONTINUE")
					{
						RequestContact = true
					}
				}
			};
			m_botClient.SendTextMessageAsync(target.TelegramUser, msg, replyMarkup:rkm);
		}

		public void SendMessage(ServerMessage message, User user)
		{
			Logger.Debug($"Telegram >> {user}: {message.Content.Substring(0, Math.Min(message.Content.Length, 32))}{(message.Content.Length > 32 ? "..." : "")}");
			m_outbox.Enqueue(new OutgoingMessage()
			{
				Message = message,
				Target = user,
			});
		}

		public void SendMessage(ServerMessage message, int userID)
		{
			Logger.Debug($"Telegram >> {userID}: {message.Content.Substring(0, Math.Min(message.Content.Length, 32))}{(message.Content.Length > 32 ? "..." : "")}");
			try
			{
				m_botClient.SendTextMessageAsync(userID, message.Content);
			}
			catch(Exception e)
			{
				//if(!e.Message.Contains("Forbidden: bot was blocked by the user"))
				{
					throw;
				}
				//var user = DodoServer.SessionManager.GetOrCreateUserFromTelegramNumber(userID);
				//user.Active = false;
			}
		}

		private async Task SendMessageAsync(OutgoingMessage msg)
		{
			if(msg == null || msg.Message == default(ServerMessage))
			{
				Logger.Warning($"Attempted to send a null message to {msg?.Target?.GetUser()}");
				return;
			}
			try
			{
				await m_botClient.SendTextMessageAsync(msg.Target.GetUser().TelegramUser, msg.Message.Content);
			}
			catch(Exception e)
			{
				if(!e.Message.Contains("Forbidden: bot was blocked by the user"))
				{
					throw;
				}
				var user = msg.Target.GetUser();
				user.Active = false;
			}
}

		public void SendMessage(ServerMessage serverMessage, IEnumerable<User> users)
		{
			foreach(var user in users)
			{
				SendMessage(serverMessage, user);
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
					// Verification recieveds TODO
					//DodoServer.SessionManager.TryVerify(e.Message.Contact.PhoneNumber, userID);
					return;
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
	}*/
}
