// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Common.Config;
using Dodo;
using Dodo.Users;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;

namespace DodoServer
{
	public class DodoStartup
	{
		public IWebHostEnvironment Environment { get; }
		public IConfiguration Configuration { get; }

		public DodoStartup(IWebHostEnvironment environment, IConfiguration configuration)
		{
			Environment = environment;
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			/*var builder = services.AddIdentityServer(config =>
			{
				config.UserInteraction.LoginUrl = $"/{UserController.RootURL}/{UserController.LOGIN}";
			})
				.AddInMemoryIdentityResources(Config.Ids)
				.AddInMemoryApiResources(Config.Apis)
				.AddInMemoryClients(Config.Clients);*/

			//builder.AddDeveloperSigningCredential();

			services.AddAuthentication(config =>
			{
				config.DefaultAuthenticateScheme = AuthConstants.AUTHSCHEME;
			})
				.AddCookie(AuthConstants.AUTHSCHEME, config =>
				{
					config.LogoutPath = $"/{UserController.RootURL}/{UserController.LOGOUT}";
					config.LoginPath = $"/{UserController.RootURL}/{UserController.LOGIN}";
					config.ExpireTimeSpan = TimeSpan.FromDays(1);
					config.SlidingExpiration = true;
				});

			services.AddAuthorization(config =>
			{
				/*config.AddPolicy("Default", config =>
				{
					config.
				})*/
			});
			services.AddTransient<IAuthorizationService, AuthService>();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseCors();
			app.UseRouting();
			app.UseStaticFiles();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
			
		}
	}
}
