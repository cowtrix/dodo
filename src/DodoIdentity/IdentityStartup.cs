// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.

using Common.Config;
using Dodo;
using Dodo.Users;
using IdentityServer4.Events;
using IdentityServer4.Models;
using IdentityServer4.Services;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.AspNetCore.Identity.MongoDB;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System.Collections.Generic;
using System.Security.Claims;
using System.Threading.Tasks;

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

			services.AddIdentityWithMongoStoresUsingCustomTypes<Microsoft.AspNetCore.Identity.MongoDB.IdentityRole>(Dodo.Dodo.PRODUCT_NAME)
				.AddDefaultTokenProviders();

			var builder = services.AddIdentityServer(options =>
				{
					options.Events.RaiseErrorEvents = true;
					options.Events.RaiseInformationEvents = true;
					options.Events.RaiseFailureEvents = true;
					options.Events.RaiseSuccessEvents = true;
					options.UserInteraction.LoginUrl = $"{UserController.RootURL}/{UserController.LOGIN}"; // leading /?
					options.UserInteraction.LogoutUrl = $"{UserController.RootURL}/{UserController.LOGOUT}";
				})
				.AddInMemoryIdentityResources(Config.Ids)
				.AddInMemoryApiResources(Config.Apis)
				.AddInMemoryClients(Config.Clients)
				.AddAspNetIdentity<User>();
			services.AddLocalApiAuthentication();
			services.AddTransient<IProfileService, ProfileService>();
#if DEBUG
			// not recommended for production - you need to store your key material somewhere secure
			builder.AddDeveloperSigningCredential();
#endif
			services.AddTransient<IAuthorizationService, AuthService>();
			services.AddAuthorization();

		}

		public void Configure(IApplicationBuilder app)
		{
			if (Environment.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
				app.UseDatabaseErrorPage();
			}
			//app.UsePathBase();
			app.UseCors();
			app.UseStaticFiles();
			app.UseRouting();
			app.UseIdentityServer(); // UseIdentityServer includes a call to UseAuthentication, so it’s not necessary to have both.
			//app.UseAuthentication();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapDefaultControllerRoute();
			});
		}
	}
}
