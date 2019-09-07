// Copyright (C) 2016 by Barend Erasmus, David Jeske and donated to the public domain

using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Configuration;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.RouteHandlers;
using System.IO;

namespace XR.Dodo
{
	public static class DodoServer
	{
		public static string TelegramSecret = "";
		public static int Port = 8080;

		public static SessionManager SessionManager = new SessionManager("sessions.json");
		public static SiteSpreadsheetManager SiteManager = new SiteSpreadsheetManager("sites.config");
		private static CoordinatorWorkflow CoordinatorWorkflow = new CoordinatorWorkflow();
		private static VolunteerWorkflow VolunteerWorkflow = new VolunteerWorkflow();
		public static SMSGateaway SMSServer;
		public static TelegramGateway TelegramGateway;
		public static User Server;

		static void Main(string[] args)
		{
			Initialise(args);
		}

		public static void Initialise(params string[] args)
		{
			log4net.Config.XmlConfigurator.Configure();

			Server = new User()
			{
				Name = "SERVER",
			};

			var secrets = File.ReadLines(@".secret").ToList();
			SMSServer = new SMSGateaway(secrets[0]);
			SMSServer.ProcessMessage = ProcessMessage;

			var route_config = new List<Route>() {
				new Route()
				{
					Name = "SMS Receiver",
					Method = "POST",
					UrlRegex = @"(?:^/)*",
					Callable = (HttpRequest request) =>
					{
						return SMSServer.Read(request);
					}
				}
			};
			var httpServer = new HttpServer(Port, route_config);
			var thread = new Thread(new ThreadStart(httpServer.Listen));
			thread.Start();

			TelegramGateway = new TelegramGateway(secrets[1]);
			TelegramGateway.ProcessMessage = ProcessMessage;
		}

		private static IEnumerable<Message> ProcessMessage(Message message, UserSession session)
		{
			session.Messages.Add(message);
			if(SiteManager.IsCoordinator(session.User))
			{
				return CoordinatorWorkflow.ProcessMessage(message, session);
			}
			return VolunteerWorkflow.ProcessMessage(message, session);
		}
	}
}
