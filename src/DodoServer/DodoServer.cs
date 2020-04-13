using Common;
using Common.Config;
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
		public static int Port => m_port.Value;
		public static string PrimaryDomain => m_domains.Value.FirstOrDefault();
		public static IEnumerable<string> Domains => m_domains.Value;
		public static string DevEmail => m_devEmail.Value;
		public static string HttpsUri => $"https://{PrimaryDomain}";
		public static string Homepage => $"https://{PrimaryDomain}";
		public static bool LetsEncryptAutoSetup => m_letsEncryptAutoSetup.Value;

		// TODO: these values don't load from file if running in IIS
		static ConfigVariable<string[]> m_domains;
		static ConfigVariable<int> m_port;
		static ConfigVariable<string> m_devEmail;
		static ConfigVariable<bool> m_letsEncryptAutoSetup = new ConfigVariable<bool>("LetsEncryptAutoSetup", false);

		private static UserTokenWorker m_tokenWorker = new UserTokenWorker();

		public static void Main(string[] args)
		{
			m_port = new ConfigVariable<int>($"{Dodo.Dodo.PRODUCT_NAME}_URI_HttpsPort", 5001);
			m_domains = new ConfigVariable<string[]>($"{Dodo.Dodo.PRODUCT_NAME}_Domain", new [] { $"localhost:{Port}" });
			m_devEmail = new ConfigVariable<string>($"{Dodo.Dodo.PRODUCT_NAME}_DevEmail", "test@web.com");
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<DodoStartup>();
					webBuilder.UseUrls(HttpsUri);
					Logger.Info($"Using HttpsUri: {HttpsUri}");
					// Workaround for HTTP2 bug in .NET Core 3.1 and Windows 8.1 / Server 2012 R2
					webBuilder.UseKestrel(options =>
						options.ConfigureEndpointDefaults(defaults =>
							defaults.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1
						)
					);
				});
	}
}
