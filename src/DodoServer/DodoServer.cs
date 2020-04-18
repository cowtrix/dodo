using Common;
using Common.Config;
using Dodo;
using Dodo.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Linq;

namespace DodoServer
{

	public static class DodoServer
	{
		public const string API_ROOT = "api/";

		public static string DevEmail => m_devEmail.Value;
		public static NetworkConfig NetConfig => m_netConfig.Value;

		// TODO: these values don't load from file if running in IIS
		static ConfigVariable<NetworkConfig> m_netConfig = new ConfigVariable<NetworkConfig>("NetworkConfig", 
			new NetworkConfig("localhost", "0.0.0.0", 5001, 5000 ));
		static ConfigVariable<string> m_devEmail = new ConfigVariable<string>($"{Dodo.Dodo.PRODUCT_NAME}_DevEmail", "test@web.com");

		//public static SessionTokenStore Sessions = new SessionTokenStore();

		public static void Main(string[] args)
		{
#if DEBUG
			Logger.Warning($"Running in Debug mode");
#endif
			SessionTokenStore.Initialise();
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<DodoStartup>();
					webBuilder.UseUrls($"https://{NetConfig.Hostname}:{NetConfig.SSLPort}", $"http://{NetConfig.Hostname}:{NetConfig.HTTPPort}");
					// Workaround for HTTP2 bug in .NET Core 3.1 and Windows 8.1 / Server 2012 R2
					webBuilder.UseKestrel(options =>
						options.ConfigureEndpointDefaults(defaults =>
							defaults.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1
						)
					);
				});
	}
}
