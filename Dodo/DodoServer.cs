using System.Collections.Generic;
using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;

namespace XR.Dodo
{
	public class Configuration
	{
		public class Spreadsheets
		{
			public class SiteSheet
			{
				public string SheetID = "";
				public int SiteNumber;
				public string SiteName = "";
			}
			public List<SiteSheet> SiteSpreadsheets = new List<SiteSheet>();
			public string CoordinatorNeedsSpreadsheetID = "";
			public string SiteSpreadsheetHealthReportID = "";
			public string WorkingGroupDataID = "";
		}

		public class Gateways
		{
			public string TwilioSID = "";
			public string TwilioAuthToken = "";
			public string TelegramGatewaySecret = "";
			public int HTTPServerPort = 8080;
			public List<Phone> Phones = new List<Phone>();
		}

		public Spreadsheets SpreadsheetData = new Spreadsheets();
		public Gateways GatewayData = new Gateways();
		public string BackupPath = "Backups";
	}

	public static class DodoServer
	{
		public static SessionManager SessionManager;
		public static SiteSpreadsheetManager SiteManager;
		public static CoordinatorNeedsManager CoordinatorNeedsManager;
		public static SMSGateway SMSGateway;
		public static TelegramGateway TelegramGateway;

		private static CmdReader cmdReader = new CmdReader();
		private static Configuration config = new Configuration();

		private static string ConfigPath { get { return Path.GetFullPath("config.json"); } }

		public static bool Dummy { get; private set; }

		static void Main(string[] args)
		{
			Initialise(args);
		}

		public static void Initialise(params string[] args)
		{
			Dummy = args.Any(x => x == "-d");

			if(!File.Exists(ConfigPath))
			{
				GenerateSampleConfig();
				return;
			}
			LoadConfig();

			if (!Directory.Exists(config.BackupPath))
			{
				Directory.CreateDirectory(config.BackupPath);
			}

			// Set up managers
			SessionManager = new SessionManager(config.BackupPath);
			SiteManager = new SiteSpreadsheetManager(config);
			CoordinatorNeedsManager = new CoordinatorNeedsManager(config.SpreadsheetData.CoordinatorNeedsSpreadsheetID);

			// Set up gateways
			SMSGateway = new SMSGateway(config.GatewayData.TwilioSID, config.GatewayData.TwilioAuthToken, config.GatewayData.HTTPServerPort);
			TelegramGateway = new TelegramGateway(config.GatewayData.TelegramGatewaySecret);
		}

		private static void GenerateSampleConfig()
		{
			Logger.Error($"Missing config file at {ConfigPath} - generating a blank one now");
			config = new Configuration();
			config.GatewayData.Phones.Add(new Phone()
			{
				Name = "Give the phone a name",
				Number = "Replace this with the phone number",
				Mode = Phone.ESMSMode.Bot,
			});
			config.SpreadsheetData.SiteSpreadsheets.Add(new Configuration.Spreadsheets.SiteSheet()
			{
				SheetID = "Put the sheet ID here",
				SiteName = "Put the name of the site here",
			});
			File.WriteAllText(ConfigPath + ".sample", JsonConvert.SerializeObject(config, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
		}

		private static void SaveConfig()
		{
			File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(config, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
		}

		private static void LoadConfig()
		{
			config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigPath), new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
		}
	}
}
