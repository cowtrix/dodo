using Common.Config;
using DodoResources;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DodoResourcesResources
{
	public static class DodoResources
	{
		public static string HttpsUrl => m_https.Value;
		static ConfigVariable<string> m_https = new ConfigVariable<string>("DodoResources_Https", "https://0.0.0.0:5000");

		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<ResourceStartup>();
					webBuilder.UseUrls($"{m_https.Value}");
				});
	}
}
