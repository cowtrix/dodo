using Common;
using Dodo;
using Dodo.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Rewrite;
using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using System;
using System.Collections.Generic;

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
			if (!Environment.IsDevelopment() && Dodo.DodoApp.NetConfig.LetsEncryptAutoSetup)
			{
				// Setup SSL certificate with Let's Encrypt
				services.AddLetsEncrypt(config =>
				{
					config.AcceptTermsOfService = true;
					config.DomainNames = Dodo.DodoApp.NetConfig.Domains;
					config.EmailAddress = DodoServer.DevEmail;
				});
			}

			services.AddSpaStaticFiles(configuration =>
			{
				configuration.RootPath = DodoServer.ReactPath;
			});

			// ICorsService and ICorsPolicyProvider are added by AddControllers... but best to be explicit in case this changes
			services.AddCors();
			services.AddControllersWithViews();
			services.AddAuthentication(config =>
			{
				config.DefaultAuthenticateScheme = AuthConstants.AUTHSCHEME;
				config.DefaultForbidScheme = AuthConstants.AUTHSCHEME;
			})
			.AddCookie(AuthConstants.AUTHSCHEME, config =>
			{
				config.LogoutPath = $"/{UserService.LOGOUT}";
				config.LoginPath = $"/{UserService.LOGIN}";
				config.AccessDeniedPath = config.LoginPath;
				config.ExpireTimeSpan = TimeSpan.FromDays(1);
				config.SlidingExpiration = true;
				config.Cookie.SameSite = SameSiteMode.None;
				config.Cookie.HttpOnly = true;
			});
			Logger.Error("REENABLE SAMESITE COOKIES BEFORE DEPLOYMENT");
			services.AddAuthorization(config =>
			{
			});
			services.AddHttpsRedirection(options =>
			{
				options.RedirectStatusCode = StatusCodes.Status307TemporaryRedirect;
				options.HttpsPort = global::Dodo.DodoApp.NetConfig.SSLPort;
			});
		}

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
			app.UseStaticFiles();
			app.UseRouting();
			app.UseHttpsRedirection();
			app.UseRewriter(GetRewriteOptions());

			// CORS must be called after UseRouting and before UseEndpoints to function correctly
			// The `Access-Control-Allow-Origin` header will not be added to normal GET responses
			// An `Origin` header must be on the request (for a different domain) for CORS to run
			// CORS headers will show on preflight OPTIONS responses if the browser uses them
			// AllowCredentials can't be used with AllowAnyOrigin (origins must be specified)
			app.UseCors(config =>
			{
				config.WithOrigins("http://localhost:3000", "https://localhost:3000").AllowCredentials() // development only
				.AllowAnyMethod()
				.AllowAnyHeader();
			});

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
			});
		}

		private RewriteOptions GetRewriteOptions()
		{
			var opt = new RewriteOptions();
			opt.AddRedirectToWwwPermanent();
			return opt;
		}
	}
}
