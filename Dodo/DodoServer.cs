using System.Linq;
using System.IO;
using System;
using Newtonsoft.Json;
using System.Threading.Tasks;
using Common;

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
		public static HTTPGateway SMSGateway;
		public static TelegramGateway TelegramGateway;
		public static Configuration Configuration = new Configuration();

		public static IMessageGateway DefaultGateway { get { return TelegramGateway; } }

		private static CmdReader cmdReader = new CmdReader();
		private static object m_loadSaveLock = new object();

		private static string ConfigPath { get { return Path.GetFullPath("config.json"); } }

		public static bool Dummy { get; private set; }
		public static bool NoLoad { get; private set; }

		static void Main(string[] args)
		{
			Initialise(args);
		}

		public static void Initialise(params string[] args)
		{
			Dummy = args.Any(x => x == "-d");
			NoLoad = args.Any(x => x == "-nl");

			if (!File.Exists(ConfigPath))
			{
				GenerateSampleConfig();
				return;
			}
			LoadConfig();

			if (!Directory.Exists(Configuration.BackupPath))
			{
				Directory.CreateDirectory(Configuration.BackupPath);
			}

			// Set up managers
			SessionManager = new SessionManager(Configuration);
			SiteManager = new SiteSpreadsheetManager(Configuration);
			CoordinatorNeedsManager = new CoordinatorNeedsManager(Configuration);

			// Set up gateways
			SMSGateway = new HTTPGateway(Configuration);
			TelegramGateway = new TelegramGateway(Configuration.GatewayData.TelegramGatewaySecret);

			var backupTask = new Task(async () =>
			{
				Logger.Debug($"Backup scheduled for every {Configuration.BackupInterval} minutes");
				while(true)
				{
					await Task.Delay(TimeSpan.FromMinutes(Configuration.BackupInterval));
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
					var existingFiles = Directory.GetFiles(Configuration.BackupPath);
					var cpyDirPath = Path.Combine(Configuration.BackupPath, DateTime.Now.ToString().Replace("/", "_").Replace(":", "_").Replace(" ", "_"));
					Directory.CreateDirectory(cpyDirPath);
					foreach (var file in existingFiles)
					{
						File.Copy(file, Path.Combine(cpyDirPath, Path.GetFileName(file)));
					}

					SessionManager.SaveToFile(Configuration.BackupPath);
					SiteManager.SaveToFile(Configuration.BackupPath);
					CoordinatorNeedsManager.SaveToFile(Configuration.BackupPath);
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
			Configuration = new Configuration();
			Configuration.GatewayData.Phones.Add(new Phone()
			{
				Name = "Give the phone a name",
				Number = "Replace this with the phone number",
			});
			Configuration.SpreadsheetData.SiteSpreadsheets.Add(new Configuration.Spreadsheets.SiteSheet()
			{
				SheetID = "Put the sheet ID here",
				SiteName = "Put the name of the site here",
			});
			File.WriteAllText(ConfigPath + ".sample", JsonConvert.SerializeObject(Configuration, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
		}

		public static void SaveConfig()
		{
			File.WriteAllText(ConfigPath, JsonConvert.SerializeObject(Configuration, Formatting.Indented, new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			}));
		}

		private static void LoadConfig()
		{
			Configuration = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(ConfigPath), new JsonSerializerSettings
			{
				TypeNameHandling = TypeNameHandling.Auto
			});
		}
	}
}
