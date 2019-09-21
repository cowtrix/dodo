using System.Collections.Generic;

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
			public string SmsSyncSecret = "";
		}

		public Spreadsheets SpreadsheetData = new Spreadsheets();
		public Gateways GatewayData = new Gateways();
		public string BackupPath = "Backups";
		public int BackupInterval = 10;
	}
}
