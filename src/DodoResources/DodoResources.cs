using Common.Config;

using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace DodoResources
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
