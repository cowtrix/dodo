using Common;
using Common.Config;
using Common.Extensions;
using Common.Security;
using Dodo;
using Dodo.Security;
using Dodo.Users;
using Dodo.Users.Tokens;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using Resources;
using System;
using System.Collections.Generic;
using System.IO;

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

			var usrManager = ResourceUtility.GetManager<User>();
			if(usrManager.Count == 0)
			{
				// Generate sysadmin account if no account is registered and print
				var schema = new UserSchema($"admin_{KeyGenerator.GetUniqueKey(6).ToLowerInvariant()}",
					ValidationExtensions.GenerateStrongPassword(), DodoApp.DevEmail);
				var user = ResourceUtility.GetFactory<User>()
					.CreateTypedObject(new ResourceCreationRequest(default, schema));

				using var rscLock = new ResourceLock(user);
				user.PersonalData.EmailConfirmed = true;
				user.TokenCollection.AddOrUpdate(user, new SysadminToken());
				usrManager.Update(user, rscLock);

				Console.BackgroundColor = ConsoleColor.Red;
				Console.ForegroundColor = ConsoleColor.Black;
				Console.WriteLine($"THIS IS IMPORTANT. A system admin account has been generated.");
				Console.WriteLine($"Username: {schema.Username}");
				Console.WriteLine($"Email: {schema.Email}");
				Console.WriteLine($"Password: {schema.Password}");
				Console.WriteLine($"THE FIRST THING YOU SHOULD DO IS CHANGE THIS PASSWORD.");
				Console.ResetColor();
			}

			CreateHostBuilder(args).Build().Run();
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
					//webBuilder.UseUrls(GetUrls().ToArray()); ;
					// Workaround for HTTP2 bug in .NET Core 3.1 and Windows 8.1 / Server 2012 R2
					webBuilder.UseKestrel(options =>
						options.ConfigureEndpointDefaults(defaults =>
							defaults.Protocols = Microsoft.AspNetCore.Server.Kestrel.Core.HttpProtocols.Http1
						)
					);
				});
	}
}
