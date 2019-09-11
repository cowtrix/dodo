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
		public TelegramGateway(string secret)
		{
			m_secret = secret;
			m_botClient = new TelegramBotClient(m_secret);
			var setup = new Task(() =>
			{
				var me = m_botClient.GetMeAsync().Result;
				Console.WriteLine($"Started Telegram bot with ID: {me.Id} and Name: {me.FirstName}.");
				m_botClient.OnMessage += Bot_OnMessage;
				m_botClient.StartReceiving();
			});
			setup.Start();
		}

		public void SendMessage(ServerMessage message, UserSession session)
		{
			var send = SendMessageAsync(message, session);
			send.Start();
		}

		async Task SendMessageAsync(ServerMessage message, UserSession session)
		{
			await m_botClient.SendTextMessageAsync(session.GetUser().TelegramUser, message.Content);
		}

		async void Bot_OnMessage(object sender, MessageEventArgs e)
		{
			if (e.Message.Text != null)
			{
				var user = DodoServer.SessionManager.GetOrCreateUserFromTelegramNumber(e.Message.From.Id);
				var session = DodoServer.SessionManager.GetOrCreateSession(user);
				var customMessage = new UserMessage(user, e.Message.Text, EGatewayType.Telegram);
				Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");
				await SendMessageAsync(session.ProcessMessage(customMessage, session), session);
			}
		}
	}
}
