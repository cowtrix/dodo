// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Common.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace DodoIdentity
{
	public static class DodoIdentity
	{
		public static string HttpsUrl => m_https.Value;
		static ConfigVariable<string> m_https = new ConfigVariable<string>("DodoIdentity_Https", "https://localhost:6000");

		public static string HttpUrl => m_http.Value;
		static ConfigVariable<string> m_http = new ConfigVariable<string>("DodoIdentity_Http", "http://localhost:6001");

		public static void Main(string[] args)
		{
			CreateHostBuilder(args).Build().Run();
		}

		public static IHostBuilder CreateHostBuilder(string[] args) =>
			Host.CreateDefaultBuilder(args)
				.ConfigureWebHostDefaults(webBuilder =>
				{
					webBuilder.UseStartup<IdentityStartup>();
					webBuilder.UseUrls($"{m_https.Value};{m_http.Value}");
				});
	}
}
