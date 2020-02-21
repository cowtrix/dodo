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
using Microsoft.Extensions.Hosting;
using IdentityServer4;
using System;

namespace DodoIdentity
{
	public class IdentityStartup
	{
		public IWebHostEnvironment Environment { get; }
		public IConfiguration Configuration { get; }

		public IdentityStartup(IWebHostEnvironment environment, IConfiguration configuration)
		{
			Environment = environment;
			Configuration = configuration;
		}

		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			var builder = services.AddIdentityServer(config =>
			{
				config.UserInteraction.LoginUrl = $"/{UserController.RootURL}/{UserController.LOGIN}";
			})
				.AddInMemoryIdentityResources(Config.Ids)
				.AddInMemoryApiResources(Config.Apis)
				.AddInMemoryClients(Config.Clients);

			builder.AddDeveloperSigningCredential();

			services.AddAuthentication(config =>
			{
			})
				.AddCookie(config =>
				{
					config.LogoutPath = $"/{UserController.RootURL}/{UserController.LOGOUT}";
					config.LoginPath = $"/{UserController.RootURL}/{UserController.LOGIN}";
					config.ExpireTimeSpan = TimeSpan.FromDays(1);
					config.SlidingExpiration = true;
				})/*.AddOpenIdConnect("oidc", options =>
				{
					options.Authority = DodoIdentity.HttpsUrl;
					options.ClientId = "spa";
				})*/;

			services.AddAuthorization(config =>
			{
				/*config.AddPolicy("Default", config =>
				{
					config.
				})*/
			});
			//services.AddTransient<IAuthorizationService, AuthService>();
		}

		public void Configure(IApplicationBuilder app)
		{
			app.UseCors();
			app.UseRouting();
			app.UseStaticFiles();
			app.UseIdentityServer();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
			
		}
	}
}