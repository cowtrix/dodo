using Common;
using Common.Commands;
using Common.Config;
using Resources;
using System.Linq;
using System.Threading;

namespace Dodo
{
	public static class DodoApp
	{
		public class EmailRedirect
		{
			public string Email { get; set; }
			public string Redirect { get; set; }
		}
		public static string DevEmail => ConfigManager.GetValue($"{Dodo.DodoApp.PRODUCT_NAME}_DevEmail", $"admin@dodo.ovh");
		public static EmailRedirect[] EmailRedirects => ConfigManager.GetValue($"{Dodo.DodoApp.PRODUCT_NAME}_EmailRedirects",
			new[] {
				new EmailRedirect{ Email = DevEmail, Redirect = "youremail@dodo.ovh" }
				});
		public const string PRODUCT_NAME = "Dodo";
		public const string API_ROOT = "api/";
		private static CommandReader m_commandReader = new CommandReader();
		public static NetworkConfig NetConfig =>
			ConfigManager.GetValue("NetworkConfig", new NetworkConfig(5001, 5000, false, "localhost", "0.0.0.0"));
	}
}
