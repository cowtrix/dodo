using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace XR.Dodo
{
	public static class DodoServer
	{
		public static string TelegramGatewaySecret { get { return m_secrets[0]; } }
		public static string CoordinatorDataID { get { return m_secrets[1]; } }
		public static string TwilioSID { get { return m_secrets[2]; } }
		public static string TwilioAuthToken { get { return m_secrets[3]; } }

		private static List<string> m_secrets;

		public static SessionManager SessionManager;
		public static SiteSpreadsheetManager SiteManager;
		public static CoordinatorNeedsManager CoordinatorNeedsManager;
		public static SMSGateway SMSGateway;
		public static TelegramGateway TelegramGateway;
		private static CmdReader cmdReader = new CmdReader();
		public static bool Dummy { get; private set; }

		static void Main(string[] args)
		{
			Initialise(args);
		}

		public static void Initialise(params string[] args)
		{
			Dummy = args.Any(x => x == "-d");
			m_secrets = File.ReadLines(@".secret").Select(x => x.Split('\t').FirstOrDefault()).ToList();

			if (!Directory.Exists("Backups"))
			{
				Directory.CreateDirectory("Backups");
			}

			// Set up managers
			SessionManager = new SessionManager("Backups\\sessions.json");
			SiteManager = new SiteSpreadsheetManager("sites.config");
			CoordinatorNeedsManager = new CoordinatorNeedsManager(CoordinatorDataID);

			// Set up gateways
			SMSGateway = new SMSGateway(TwilioSID, TwilioAuthToken, 4096);
			TelegramGateway = new TelegramGateway(TelegramGatewaySecret);
		}
	}
}
