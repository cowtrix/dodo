using Common.Config;
using Common.Extensions;
using Common.Security;
using System;
using System.Collections.Concurrent;
using System.IO;
using System.Security;
using System.Security.Cryptography;
using System.Threading.Tasks;

namespace Common
{
	public interface IBackup
	{
		string BackupPath { get; }
		string Serialize();
		void Deserialize(string json);
	}


	public class BackupManager
	{
		public ConfigVariable<int> BackupInterval = new ConfigVariable<int>("BackupInterval", 5);
		public ConfigVariable<string> BackupLocation = new ConfigVariable<string>("BackupLocation", "Backups");
		public ConfigVariable<bool> Encrypt = new ConfigVariable<bool>("EncryptBackups", false);
		ConcurrentDictionary<IBackup, Task> m_backups = new ConcurrentDictionary<IBackup, Task>();
		private SecureString m_pass;

		public BackupManager()
		{
			if(!Directory.Exists(BackupLocation.Value))
			{
				Directory.CreateDirectory(BackupLocation.Value);
			}
			if (Encrypt.Value)
			{
				Console.Write("Enter data password: ");
				m_pass = ConsoleExtensions.ReadPassword();
			}
		}

		public BackupManager RegisterBackup(IBackup backup)
		{
			m_backups[backup] = new Task(async () =>
			{
				while(true)
				{
					var target = backup;
					var path = Path.GetFullPath(Path.Combine(BackupLocation.Value, target.BackupPath + ".json"));
					if (File.Exists(path))
					{
						var data = File.ReadAllText(path);
						if (Encrypt.Value)
						{
							data = SymmetricSecurity.Decrypt<string>(data, m_pass.ToString());
						}
					}
					await Task.Delay(TimeSpan.FromMinutes(BackupInterval.Value));
					try
					{
						var data = target.Serialize();
						if(Encrypt.Value)
						{
							data = SymmetricSecurity.Encrypt(data, m_pass.ToString());
						}
						File.WriteAllText(path, data);
					}
					catch(Exception e)
					{
						Logger.Exception(e, $"Exception in backup for {target?.GetType()}");
					}
				}
			});
			m_backups[backup].Start();
			return this;
		}
	}
}
