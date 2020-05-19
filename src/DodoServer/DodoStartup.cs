using Common;
using Dodo;
using Dodo.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Extensions;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.SpaServices.ReactDevelopmentServer;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
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
			if (!Environment.IsDevelopment() && DodoServer.NetConfig.LetsEncryptAutoSetup)
			{
				// Setup SSL certificate with Let's Encrypt
				services.AddLetsEncrypt(config =>
				{
					config.AcceptTermsOfService = true;
					config.DomainNames = new[] { DodoServer.NetConfig.Domain };
					config.EmailAddress = DodoServer.DevEmail;
				});
			}

			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = DodoServer.ReactPath;
			});

#if DEBUG
			// ICorsService and ICorsPolicyProvider are added by AddControllers... but best to be explicit in case this changes
			services.AddCors();
#endif
			services.AddControllersWithViews();

			services.AddAuthentication(config =>
			{
				config.DefaultAuthenticateScheme = AuthConstants.AUTHSCHEME;
				config.DefaultForbidScheme = AuthConstants.AUTHSCHEME;
			})
			.AddCookie(AuthConstants.AUTHSCHEME, config =>
			{
				config.LogoutPath = $"/{UserController.LOGOUT}";
				config.LoginPath = $"/{UserController.LOGIN}";
				config.AccessDeniedPath = config.LoginPath;
				config.ExpireTimeSpan = TimeSpan.FromDays(1);
				config.SlidingExpiration = true;
				config.Cookie.SameSite = SameSiteMode.Strict;
				config.Cookie.HttpOnly = true;
			});
			services.AddAuthorization(config =>
			{
			});
			services.AddHttpsRedirection(options =>
			{
				options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
				options.HttpsPort = DodoServer.NetConfig.SSLPort;
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseRouting();

#if DEBUG
			// CORS must be called after UseRouting and before UseEndpoints to function correctly
			// The `Access-Control-Allow-Origin` header will not be added to normal GET responses
			// An `Origin` header must be on the request (for a different domain) for CORS to run
			// CORS headers will show on preflight OPTIONS responses if the browser uses them
			// AllowCredentials can't be used with AllowAnyOrigin (origins must be specified)
			app.UseCors(config =>
			{
				config.WithOrigins("http://localhost:3000").AllowCredentials() // development only
				.AllowAnyMethod()
				.AllowAnyHeader();
			});
#endif

			app.UseStaticFiles();
			app.UseSpaStaticFiles();
			app.UseAuthentication();
			app.UseAuthorization();
			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
				endpoints.MapDefaultControllerRoute();
			});
			app.UseSpa(spa =>
			{
				spa.Options.SourcePath = DodoServer.ReactPath;
				spa.ApplicationBuilder.Use(async (context, next) =>
				{
					if(context.Request.Scheme != "https")
					{
						context.Request.Scheme = "https";
						if(context.Request.Host.Port.HasValue)
						{
							context.Request.Host = new HostString(context.Request.Host.Host, DodoServer.NetConfig.SSLPort);
						}
						context.Response.StatusCode = (int)System.Net.HttpStatusCode.MovedPermanently;
						context.Response.Headers.Add("Location", context.Request.GetEncodedUrl());
						await context.Response.WriteAsync("Redirect");
						return;
					}
					await next.Invoke();
				});
			});
			app.UseHttpsRedirection();
		}
	}
}
