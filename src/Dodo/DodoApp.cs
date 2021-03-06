using Common;
using Common.Commands;
using Common.Config;
using Common.Security;
using Resources;
using System.IO;
using System.Linq;
using System.Threading;

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
		public static string PrivacyPolicyURL => ConfigManager.GetValue("PrivacyPolicyURL", $"https://extinctionrebellion.uk/privacy-policy/");
		public static string RebelAgreementURL => ConfigManager.GetValue("RebelAgreementURL", $"https://extinctionrebellion.uk/wp-content/uploads/2019/04/Extinction-Rebellion-Rebel-Agreement.pdf");
		public static string DevEmail => ConfigManager.GetValue($"{Dodo.DodoApp.PRODUCT_NAME}_DevEmail", $"admin@dodo.ovh");
		public static string ServerSalt { get; set; }
		public const string PRODUCT_NAME = "Dodo";
		public const string API_ROOT = "api/";
		private static CommandReader m_commandReader = new CommandReader();
		public static NetworkConfig NetConfig =>
			ConfigManager.GetValue("NetworkConfig", new NetworkConfig(5001, 5000, false, "localhost", "0.0.0.0"));
		public static string WebRoot { get; set; } = "./wwwroot";

		static DodoApp()
		{
			var saltPath = Path.GetFullPath("salt");
			if(File.Exists(saltPath))
			{
				ServerSalt = File.ReadAllText(saltPath);
			}
			else
			{
				ServerSalt = KeyGenerator.GetUniqueKey(256);
				File.WriteAllText(saltPath, ServerSalt);
			}
		}
	}
}
