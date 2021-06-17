using Common;
using Common.Commands;
using Common.Config;
using Common.Security;
using Resources;
using System.IO;

namespace Dodo
{
	public static class DodoApp
	{
		public class EmailRedirect
		{
			[Email]
			public string Email { get; set; }
			[Email]
			public string Redirect { get; set; }
		}

		static PersistentStore<string, string> m_configStore = new PersistentStore<string, string>("auth", "config");

		public static string PrivacyPolicyURL => ConfigManager.GetValue("PrivacyPolicyURL", "rsc/privacypolicy");
		public static string RebelAgreementURL => ConfigManager.GetValue("RebelAgreementURL", "rsc/rebelagreement");
		public static string SupportEmail => ConfigManager.GetValue($"{Dodo.DodoApp.PRODUCT_NAME}_DevEmail", $"support@{NetConfig.Domains[0]}");
		public static string ServerSalt { get; private set; }
		public const string PRODUCT_NAME = "resistance.earth";
		public const string API_ROOT = "api/";
		private static CommandReader m_commandReader = new CommandReader();
		public static NetworkConfig NetConfig =>
			ConfigManager.GetValue("NetworkConfig", new NetworkConfig(5001, 22957, false, "localhost", "0.0.0.0"));
		public static string WebRoot { get; set; } = "./wwwroot";
		public static string ContentRoot { get; set; } = "./";

		static DodoApp()
		{
			if (m_configStore.TryGetValue(nameof(ServerSalt), out var salt) && !string.IsNullOrEmpty(ServerSalt) && ServerSalt.Length == 128)
			{
				ServerSalt = salt; 
			}
			else
			{
				ServerSalt = KeyGenerator.GetUniqueKey(128);
				m_configStore[nameof(ServerSalt)] = ServerSalt;
				Logger.Debug($"Generated server salt: {ServerSalt}");
			}
		}
	}
}
