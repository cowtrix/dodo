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
		public static string SMSGatewaySecret { get { return m_secrets[0]; } }
		public static string TelegramGatewaySecret { get { return m_secrets[1]; } }
		public static string CoordinatorDataID { get { return m_secrets[2]; } }

		private static List<string> m_secrets;
		

		public static SessionManager SessionManager;
		public static SiteSpreadsheetManager SiteManager;
		public static CoordinatorNeedsManager CoordinatorNeedsManager;
		public static SMSGateaway SMSGateway;
		public static TelegramGateway TelegramGateway;

		static void Main(string[] args)
		{
			Initialise(args);
		}

		public static void Initialise(params string[] args)
		{
			m_secrets = File.ReadLines(@".secret").Select(x => x.Split('\t').FirstOrDefault()).ToList();
			log4net.Config.XmlConfigurator.Configure();

			// Set up managers
			SessionManager = new SessionManager("sessions.json");
			SiteManager = new SiteSpreadsheetManager("sites.config");
			CoordinatorNeedsManager = new CoordinatorNeedsManager(CoordinatorDataID);
			
			// Set up gateways
			SMSGateway = new SMSGateaway(SMSGatewaySecret, 8080);
			TelegramGateway = new TelegramGateway(TelegramGatewaySecret);
		}
	}
}
