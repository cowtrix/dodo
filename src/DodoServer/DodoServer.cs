using Common;
using Common.Config;
using Dodo.Security;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DodoServer
{
	public static class DodoServer
	{
		public static string DevEmail => ConfigManager.GetValue($"{Dodo.DodoApp.PRODUCT_NAME}_DevEmail", "test@web.com");
		public static string ReactPath => ConfigManager.GetValue("ReactPath", Path.GetFullPath(@"..\..\dodo-frontend\build"));

		public static void Main(string[] args)
		{
#if DEBUG
			Logger.Warning($"Running in Debug mode");
#endif
			SessionTokenStore.Initialise();
			CreateHostBuilder(args).Build().Run();
		}

		private static IEnumerable<string> GetUrls()
		{
			var config = Dodo.DodoApp.NetConfig;
			foreach (var d in config.Domains)
			{
				yield return d;
			}
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<DodoStartup>();
					webBuilder.UseUrls(GetUrls().ToArray()); ;
					// Workaround for HTTP2 bug in .NET Core 3.1 and Windows 8.1 / Server 2012 R2
					webBuilder.UseKestrel(options =>
						options.ConfigureEndpointDefaults(defaults =>
							defaults.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1
						)
					);
				});
	}
}
