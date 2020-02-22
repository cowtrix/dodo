using Microsoft.AspNetCore.Builder;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Hosting;
using Microsoft.AspNetCore.Authentication.Certificate;
using System;
using System.IdentityModel.Tokens.Jwt;
using System.Threading.Tasks;
using Common.Config;

namespace DodoResources
{
    public class ResourceStartup
	{
        static ConfigVariable<string> m_authURI = new ConfigVariable<string>("DodoIdentity_Https", "https://0.0.0.0:6000");

        public void ConfigureServices(IServiceCollection services)
		{
			services.AddControllers();
            services.AddCors();
            services.AddDistributedMemoryCache();

            services.AddAuthentication("cookie")
                .AddIdentityServerAuthentication()
                .AddCookie("cookie")
                .AddCertificate(options =>
                {
                    options.AllowedCertificateTypes = CertificateTypes.All;
                });
        }

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
            app.UseCors(policy =>
            {
                policy.WithOrigins(
                    DodoResources.HttpsUrl,
                    m_authURI.Value);

                policy.AllowAnyHeader();
                policy.AllowAnyMethod();
                policy.WithExposedHeaders("WWW-Authenticate");
            });

            if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
			}

			app.UseRouting();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}