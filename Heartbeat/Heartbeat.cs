using Common;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net.Http;
using System.Net.NetworkInformation;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using Telegram.Bot;
using Telegram.Bot.Args;

namespace XR.Heartbeat
{
	public class Configuration
	{
		public string PingAddress;
		public string GetAddress;
		public string TelegramSecret;
		public string Password;
		public int PingInterval;
		public List<int> AuthorisedUsers = new List<int>();
	}

	class Heartbeat
	{
		static Configuration m_configuration;
		static TelegramBotClient m_botClient;
		static string m_configPath;
		static HttpClient m_client = new HttpClient();
		static DateTime m_lastCheck;

		static void Main(string[] args)
		{
			m_configPath = Path.GetFullPath(args[0]);
			if(!File.Exists(m_configPath))
			{
				m_configuration = new Configuration();
				SaveConfig();
				return;
			}
			LoadConfig();
			var workerThread = new Task(async () =>
			{
				try
				{
					m_botClient = new TelegramBotClient(m_configuration.TelegramSecret);
					var me = m_botClient.GetMeAsync().Result;
					Console.WriteLine($"Started Telegram bot with ID: {me.Id}, User {me.Username} and Name: {me.FirstName}.");
					m_botClient.OnMessage += Bot_OnMessage;
					m_botClient.StartReceiving();
				}
				catch (Exception e)
				{
					Console.WriteLine("Failed to start Telegram Bot. " + e.Message);
				}

				while(true)
				{
					try
					{
						Thread.Sleep(TimeSpan.FromMinutes(m_configuration.PingInterval));
						Ping ping = new Ping();
						PingReply pingresult = ping.Send(m_configuration.PingAddress);
						if (pingresult.Status.ToString() != "Success")
						{
							OnError($"Ping to {m_configuration.PingAddress} failed with error code: {pingresult.Status.ToString()}");
							continue;
						}
						var httpReq = await GET(m_configuration.GetAddress);
						if(!httpReq.IsSuccessStatusCode)
						{
							OnError($"GET request to {m_configuration.PingAddress} failed with error code: {httpReq.StatusCode} {httpReq.ReasonPhrase}");
							continue;
						}
						var content = await httpReq.Content.ReadAsStringAsync();
						var exceptions = JsonConvert.DeserializeObject<List<Logger.ExceptionEntry>>(content);
						var newExceptions = exceptions.Where(x => x.TimeStamp >= m_lastCheck);
						if(newExceptions.Any())
						{
							OnError($"Exceptions received from {m_configuration.PingAddress}");
							foreach(var exc in newExceptions)
							{
								OnError($"{exc.TimeStamp}: {exc.Message}");
							}
						}
						m_lastCheck = DateTime.Now;
					}
					catch (Exception e)
					{
						Console.WriteLine("Heartbeat had an exception: " + e.Message);
					}
				}
			});
			workerThread.Start();
			while (true) ;
		}

		static void OnError(string errorMessage)
		{
			foreach (var user in m_configuration.AuthorisedUsers)
			{
				m_botClient.SendTextMessageAsync(user, "ERROR: " + errorMessage);
			}
		}

		static void SaveConfig()
		{
			File.WriteAllText(m_configPath, JsonConvert.SerializeObject(new Configuration(), Formatting.Indented));
			Console.WriteLine("Saved config file at " + m_configPath);
		}

		static void LoadConfig()
		{
			m_configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(m_configPath));
			Console.WriteLine("Loaded config file at " + m_configPath);
		}

		private static void Bot_OnMessage(object sender, MessageEventArgs e)
		{
			var message = e.Message.Text;
			var userID = e.Message.From.Id;
			if(message == "/start")
			{
				if (m_configuration.AuthorisedUsers.Contains(userID))
				{
					m_botClient.SendTextMessageAsync(userID, "You're already authorised.");
				}
				else
				{
					m_botClient.SendTextMessageAsync(userID, "Enter password");
				}
			}
			if (message == "Unsubscribe" && m_configuration.AuthorisedUsers.Remove(userID))
			{
				m_botClient.SendTextMessageAsync(userID, "You've unsubscribed from updates.");
				return;
			}
			if (message == m_configuration.Password)
			{
				if(m_configuration.AuthorisedUsers.Contains(userID))
				{
					m_botClient.SendTextMessageAsync(userID, "You're already authorised.");
					return;
				}
				m_configuration.AuthorisedUsers.Add(userID);
				SaveConfig();
				m_botClient.SendTextMessageAsync(userID, "You were already authorised");
			}
			else
			{
				m_botClient.SendTextMessageAsync(userID, "Incorrect password");
			}
		}

		static async Task<string> POST(string address, Dictionary<string, string> values)
		{
			var content = new FormUrlEncodedContent(values);
			var response = await m_client.PostAsync(address, content);
			return await response.Content.ReadAsStringAsync();
		}

		static async Task<HttpResponseMessage> GET(string address)
		{
			return await m_client.GetAsync(address);
		}
	}
}
