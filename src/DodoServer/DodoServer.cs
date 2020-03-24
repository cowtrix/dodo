using Common.Config;
using Dodo.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DodoServer
{
	public static class DodoServer
	{
		public const string API_ROOT = "api/";
		public static int Port => m_port.Value;
		public static string HttpsUrl => $"{m_url.Value}:{m_port.Value}";
		public static string Homepage => m_index.Value;

		// TODO: these values don't load from file if running in IIS
		static ConfigVariable<string> m_url = new ConfigVariable<string>($"{Dodo.Dodo.PRODUCT_NAME}URI_Https", "https://localhost");
		static ConfigVariable<int> m_port = new ConfigVariable<int>($"{Dodo.Dodo.PRODUCT_NAME}URI_HttpsPort", 5001);
		static ConfigVariable<string> m_index = new ConfigVariable<string>($"{Dodo.Dodo.PRODUCT_NAME}URI_Index", $"{m_url.Value}:{m_port.Value}/api");

		private static UserTokenWorker m_tokenWorker = new UserTokenWorker();

		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<DodoStartup>();
					webBuilder.UseUrls(HttpsUrl);
					// Workaround for HTTP2 bug in .NET Core 3.1 and Windows 8.1 / Server 2012 R2
					webBuilder.UseKestrel(options =>
						options.ConfigureEndpointDefaults(defaults =>
							defaults.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1
						)
					);
				});
	}
}
