using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;

namespace XR.Dodo
{
	public static class DodoServer
	{
		public static string SMSGatewaySecret { get { return m_secrets[0]; } }
		public static string TelegramGatewaySecret { get { return m_secrets[1]; } }
		public static string CoordinatorDataID { get { return m_secrets[2]; } }
		public static string TwilioSID { get { return m_secrets[3]; } }
		public static string TwilioAuthToken { get { return m_secrets[4]; } }
		public static string TwilioNumber { get { return m_secrets[5]; } }

		private static List<string> m_secrets;

		public static SessionManager SessionManager;
		public static SiteSpreadsheetManager SiteManager;
		public static CoordinatorNeedsManager CoordinatorNeedsManager;
		public static SMSGateaway SMSGateway;
		public static TelegramGateway TelegramGateway;
		public static TwilioGateway TwilioGateway;
		private static CmdReader cmdReader = new CmdReader();
		public static bool Dummy { get; private set; }

		public static string GetSMSNumber()
		{
			return SMSGateway.GetNumber();
		}

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
			SessionManager = new SessionManager("sessions.json");
			SiteManager = new SiteSpreadsheetManager("sites.config");
			CoordinatorNeedsManager = new CoordinatorNeedsManager(CoordinatorDataID);

			SessionManager.GetOrCreateUserFromTelegramNumber(834876848).CoordinatorRoles.Add(
				SiteManager.GetSites().ElementAt(2).WorkingGroups.First());

			// Set up gateways
			SMSGateway = new SMSGateaway(SMSGatewaySecret, 8080);
			TelegramGateway = new TelegramGateway(TelegramGatewaySecret);
			TwilioGateway = new TwilioGateway(TwilioSID, TwilioAuthToken, TwilioNumber);
		}

		public static void SendSMS(ServerMessage message, string phoneNumber)
		{
			TwilioGateway.SendMessage(message, phoneNumber);
		}
	}
}
