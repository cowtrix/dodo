using System;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace XR.Dodo
{
	public class TelegramGateway : IMessageGateway
	{
		readonly string m_secret;
		TelegramBotClient m_botClient;

		public EGatewayType Type { get { return EGatewayType.Telegram; } }

		public string UserName { get; private set; }

		public TelegramGateway(string secret)
		{
			m_secret = secret;
			if (DodoServer.Dummy)
			{
				return;
			}
			var setup = new Task(() =>
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
			});
			setup.Start();
		}

		public void SendMessage(ServerMessage message, UserSession session)
		{
			var t = new Task(async () =>
			{
				try
				{
					await SendMessageAsync(message, session);
				}
				catch(Exception e)
				{
					Logger.Exception(e, $"Failed to send telegram message to {session.UserID}: " + message.Content);
				}
			});
			t.Start();
		}

		async Task SendMessageAsync(ServerMessage message, UserSession session)
		{
			if(message.Content.Length > 160)
			{
				Logger.Warning("Message length > sms limit");
			}
			await m_botClient.SendTextMessageAsync(session.GetUser().TelegramUser, message.Content);
		}

		async void Bot_OnMessage(object sender, MessageEventArgs e)
		{
			try
			{
				var message = e.Message.Text;
				var userID = e.Message.From.Id;
				await SendMessageAsync(GetMessage(message, userID, out var session), session);
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
				var customMessage = new UserMessage(user, message, this, userID.ToString());
				return session.ProcessMessage(customMessage, session);
			}
			return default(ServerMessage);
		}

		public async Task<ServerMessage> FakeMessage(string message, int userID)
		{
			return GetMessage(message, userID, out var _);
		}
	}
}
