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

	public struct ConfigVariable<T>
	{
		public T Value;
		public string ConfigKey;
		public ConfigVariable(string key, T defaultValue)
		{
			Value = defaultValue;
			ConfigKey = key;
		}
	}

	public class BackupManager
	{
		public ConfigVariable<int> BackupInterval = new ConfigVariable<int>("BackupInterval", 5);
		public ConfigVariable<string> BackupLocation = new ConfigVariable<string>("BackupLocation", "Backups");
		public ConfigVariable<bool> Encrypt = new ConfigVariable<bool>("EncryptBackups", true);
		ConcurrentDictionary<IBackup, Task> m_backups = new ConcurrentDictionary<IBackup, Task>();

		public BackupManager()
		{
			if(!Directory.Exists(BackupLocation.Value))
			{
				Directory.CreateDirectory(BackupLocation.Value);
			}
		}

		public void RegisterBackup(IBackup backup)
		{
			m_backups[backup] = new Task(async () =>
			{
				while(true)
				{
					var target = backup;
					var path = Path.GetFullPath(Path.Combine(BackupLocation.Value, target.BackupPath + ".json"));
					var data = File.ReadAllText(path);
					var pass = "";
					if (Encrypt.Value)
					{
						Console.Write("Enter decryption password: ");
						pass = ConsoleExtensions.ReadPassword();
						data = StringCipher.Decrypt(data, pass);
					}
					await Task.Delay(TimeSpan.FromMinutes(BackupInterval.Value));
					try
					{
						data = target.Serialize();
						if(Encrypt.Value)
						{
							data = StringCipher.Encrypt(data, pass);
						}
						File.WriteAllText(path, data);
					}
					catch(Exception e)
					{
						Logger.Exception(e, $"Exception in backup for {target?.GetType()}");
					}
				}
			});
		}
	}
}
