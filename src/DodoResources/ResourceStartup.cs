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

            services.AddAuthentication()
                .AddIdentityServerAuthentication()
                .AddCertificate(options =>
                {
                    options.AllowedCertificateTypes = CertificateTypes.All;
                });
        }

		public void Configure(IApplicationBuilder app, IWebHostEnvironment env)
		{
            if (env.IsDevelopment())
			{
				app.UseDeveloperExceptionPage();
                app.UseCors(policy =>
                {
                    policy.AllowAnyOrigin()
                    .AllowCredentials()
                    .AllowAnyHeader()
                    .AllowAnyMethod();
                });
            }

			app.UseRouting();

            app.UseAuthentication();

			app.UseEndpoints(endpoints =>
			{
				endpoints.MapControllers();
			});
		}
	}
}