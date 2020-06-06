using Common;
using Common.Commands;
using Common.Config;
using Resources;
using System.Threading;

namespace Dodo
{
	public static class Dodo
	{
		public const string PRODUCT_NAME = "Dodo";
		public const string API_ROOT = "api/";
		private static CommandReader m_commandReader = new CommandReader();
		public static NetworkConfig NetConfig => ConfigManager.GetValue("NetworkConfig", new NetworkConfig("localhost", "0.0.0.0", 5001, 5000));
	}
}
