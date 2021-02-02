using Common;
using Common.Config;
using Common.Extensions;
using Common.Security;
using Dodo;
using Dodo.Expiry;
using Dodo.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace DodoServer
{
	public static class DodoServer
	{
		public static string ReactPath => ConfigManager.GetValue("ReactPath", Path.GetFullPath(@"..\..\dodo-frontend\build"));

		public static void Main(string[] args)
		{
#if DEBUG
			Logger.Warning($"Running in Debug mode");
#endif
			SessionTokenStore.Initialise();
			ExpiryWorker.Initialise();

			var usrManager = ResourceUtility.GetManager<User>();
			if(usrManager.Count == 0)
			{
				GenerateSysadmin();
			}

			CreateHostBuilder(args).Build().Run();
		}

		private static void GenerateSysadmin()
		{
			// Generate sysadmin account if no account is registered and print
			var schema = new UserSchema($"admin_{KeyGenerator.GetUniqueKey(6).ToLowerInvariant()}", 
				ValidationExtensions.GenerateStrongPassword(), DodoApp.DevEmail);
			ResourceUtility.GetFactory<User>()
				.CreateTypedObject(new ResourceCreationRequest(default, schema));
			Console.BackgroundColor = ConsoleColor.Red;
			Console.ForegroundColor = ConsoleColor.Black;
			Console.WriteLine($"THIS IS IMPORTANT. A system admin account has been generated.");
			Console.WriteLine($"Username: {schema.Username}");
			Console.WriteLine($"Email: {schema.Email}");
			Console.WriteLine($"Password: {schema.Password}");
			Console.WriteLine($"THE FIRST THING YOU SHOULD DO IS CHANGE THIS PASSWORD.");
			Console.ResetColor();
		}

		private static IEnumerable<string> GetUrls()
		{
			var config = Dodo.DodoApp.NetConfig;
			foreach (var d in config.Domains)
			{
				yield return $"https://{d}:{config.SSLPort}";
				yield return $"http://{d}:{config.HTTPPort}";
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
