using Microsoft.AspNetCore.Authentication;
using Microsoft.AspNetCore.Authentication.JwtBearer;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Identity;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using REST;
using System;
using System.Collections.Generic;
using System.Security.Claims;
using Microsoft.AspNetCore.Authorization;
using Dodo;
using Microsoft.IdentityModel.Logging;
using System.Net;

namespace DodoResources
{
    public class ResourceStartup
	{
		public ResourceStartup(IConfiguration configuration)
		{
			Configuration = configuration;
		}

		public IConfiguration Configuration { get; }

		// This method gets called by the runtime. Use this method to add services to the container.
		public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
			services.AddIdentityWithMongoStoresUsingCustomTypes<Microsoft.AspNetCore.Identity.MongoDB.IdentityRole>(Dodo.Dodo.PRODUCT_NAME)
				.AddDefaultTokenProviders();
			services.AddRouting(options => options.LowercaseUrls = true);
			services.AddAuthentication("IdentityServer4")
				.AddIdentityServerAuthentication("IdentityServer4", options =>
				{
					//options.Authority = ;
					options.RequireHttpsMetadata = false;
					options.ApiName = "api";
				});

			services.AddTransient<IAuthorizationService, AuthService>();
			services.AddAuthorization();
		}

		// This method gets called by the runtime. Use this method to configure the HTTP request pipeline.
		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseAuthentication();

			app.UseAuthorization();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}