using System;
using System.Collections.Generic;
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

		public Func<Message, UserSession, IEnumerable<Message>> ProcessMessage { get; set; }

		async void Bot_OnMessage(object sender, MessageEventArgs e)
		{
			if (e.Message.Text != null)
			{
				var session = DodoServer.SessionManager.GetOrCreateSessionFromTelegramName(e.Message.From.Id);
				var customMessage = new Message(session.User, e.Message.Text);
				Console.WriteLine($"Received a text message in chat {e.Message.Chat.Id}.");
				var responses = ProcessMessage(customMessage, session);
				foreach(var response in responses)
				{
					await m_botClient.SendTextMessageAsync(
					  chatId: e.Message.Chat,
					  text: response.Content
					);
				}
			}
		}
	}
}
