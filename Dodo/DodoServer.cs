using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public interface IBackup
	{
		void SaveToFile(string path);
		void LoadFromFile(string path);
	}

	public static class DodoServer
	{
		public static SessionManager SessionManager;
		public static SiteSpreadsheetManager SiteManager;
		public static CoordinatorNeedsManager CoordinatorNeedsManager;
		public static SMSGateway SMSGateway;
		public static TelegramGateway TelegramGateway;

		private static CmdReader cmdReader = new CmdReader();
		private static Configuration m_configuration = new Configuration();

		private static object m_loadSaveLock = new object();

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

			if (!Directory.Exists(m_configuration.BackupPath))
			{
				Directory.CreateDirectory(m_configuration.BackupPath);
			}

			// Set up managers
			SessionManager = new SessionManager(m_configuration);
			SiteManager = new SiteSpreadsheetManager(m_configuration);
			CoordinatorNeedsManager = new CoordinatorNeedsManager(m_configuration);

			// Set up gateways
			SMSGateway = new SMSGateway(m_configuration.GatewayData.TwilioSID, m_configuration.GatewayData.TwilioAuthToken, m_configuration.GatewayData.HTTPServerPort);
			TelegramGateway = new TelegramGateway(m_configuration.GatewayData.TelegramGatewaySecret);

			var backupTask = new Task(() =>
			{
				Logger.Debug($"Backup scheduled for every {m_configuration.BackupInterval} minutes");
				while(true)
				{
					System.Threading.Thread.Sleep(TimeSpan.FromMinutes(m_configuration.BackupInterval));
					Backup();
				}
			});
			backupTask.Start();
		}

		public static void Backup()
		{
			try
			{
				lock(m_loadSaveLock)
				{
					var existingFiles = Directory.GetFiles(m_configuration.BackupPath);
					var cpyDirPath = Path.Combine(m_configuration.BackupPath, DateTime.Now.ToString().Replace("/", "_").Replace(":", "_").Replace(" ", "_"));
					Directory.CreateDirectory(cpyDirPath);
					foreach (var file in existingFiles)
					{
						File.Copy(file, Path.Combine(cpyDirPath, Path.GetFileName(file)));
					}

					SessionManager.SaveToFile(m_configuration.BackupPath);
					SiteManager.SaveToFile(m_configuration.BackupPath);
					CoordinatorNeedsManager.SaveToFile(m_configuration.BackupPath);
				}
				Logger.Debug("All backups saved");
			}
			catch (Exception e)
			{
				Logger.Exception(e, "Backup failed!");
			}
		}

		public static void Load(string directory)
		{
			try
			{
				directory = Path.GetFullPath(directory);
				if (!Directory.Exists(directory))
				{
					throw new DirectoryNotFoundException(directory);
				}
				lock (m_loadSaveLock)
				{
					SessionManager.LoadFromFile(directory);
					SiteManager.LoadFromFile(directory);
					CoordinatorNeedsManager.LoadFromFile(directory);
				}
				Logger.Debug("All backups loaded");
			}
			catch (Exception e)
			{
				Logger.Exception(e, $"Load of backup at {directory} failed!");
			}
		}

		private static void GenerateSampleConfig()
		{
			Logger.Error($"Missing config file at {ConfigPath} - generating a blank one now");
			m_configuration = new Configuration();
			m_configuration.GatewayData.Phones.Add(new Phone()
			{
				Name = "Give the phone a name",
				Number = "Replace this with the phone number",
				Mode = Phone.ESMSMode.Bot,
			});
			m_configuration.SpreadsheetData.SiteSpreadsheets.Add(new Configuration.Spreadsheets.SiteSheet()
			{
				SheetID = "Put the sheet ID here",
				SiteName = "Put the name of the site here",
			});
			File.WriteAllText(ConfigPath + ".sample", JsonConvert.SerializeObject(m_configuration, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
		}

		private static void SaveConfig()
		{
			File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(m_configuration, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
		}

		private static void LoadConfig()
		{
			m_configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigPath), new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
		}
	}
}
