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
		public static string DevEmail => ConfigManager.GetValue($"{Dodo.DodoApp.PRODUCT_NAME}_DevEmail", $"admin@{NetConfig.Domains.First()}");
		public static System.ValueTuple<string, string>[] EmailRedirects => ConfigManager.GetValue($"{Dodo.DodoApp.PRODUCT_NAME}_EmailRedirects", 
			new[]{
				( DevEmail, "youremailhere@example.com" )
				});
		public const string PRODUCT_NAME = "Dodo";
		public const string API_ROOT = "api/";
		private static CommandReader m_commandReader = new CommandReader();
		public static NetworkConfig NetConfig => 
			ConfigManager.GetValue("NetworkConfig", new NetworkConfig(5001, 5000, false, "localhost", "0.0.0.0"));
	}
}
