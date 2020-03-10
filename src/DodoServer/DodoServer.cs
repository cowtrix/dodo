// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using Common.Config;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Linq;

namespace DodoServer
{
	public static class DodoServer
	{
		public static int Port => m_port.Value;
		public static string HttpsUrl => $"{m_url.Value}:{m_port.Value}";
		public static string Index => m_index.Value;

		static ConfigVariable<string> m_url = new ConfigVariable<string>($"{Dodo.Dodo.PRODUCT_NAME}URI_Https", "https://0.0.0.0");
		static ConfigVariable<int> m_port = new ConfigVariable<int>($"{Dodo.Dodo.PRODUCT_NAME}URI_HttpsPort", 5001);
		static ConfigVariable<string> m_index = new ConfigVariable<string>($"{Dodo.Dodo.PRODUCT_NAME}URI_Index", m_url.Value);

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
				});
	}
}
