using Dodo;
using Dodo.Users;
using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.Http;
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
			if(!Environment.IsDevelopment() && DodoServer.NetConfig.LetsEncryptAutoSetup)
			{
				services.AddLetsEncrypt(config =>
				{
					config.AcceptTermsOfService = true;
					config.DomainNames = new[] { DodoServer.NetConfig.Domain };
					config.EmailAddress = DodoServer.DevEmail;
				});
			}
			
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
				config.LogoutPath = $"/{UserController.RootURL}/{UserController.LOGOUT}";
				config.LoginPath = $"/{UserController.RootURL}/{UserController.LOGIN}";
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

		public void Configure(IApplicationBuilder app)
		{
			app.UseHttpsRedirection();
			app.UseRouting();

			// CORS must be called after UseRouting and before UseEndpoints to function correctly
			// The `Access-Control-Allow-Origin` header will not be added to normal GET responses
			// An `Origin` header must be on the request (for a different domain) for CORS to run
			// CORS headers will show on preflight OPTIONS responses if the browser uses them
			// AllowCredentials can't be used with AllowAnyOrigin (origins must be specified)
			app.UseCors(config =>
			{
				config.AllowAnyOrigin()
				.AllowAnyMethod()
				.AllowAnyHeader();
			});

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
