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

            services.AddAuthentication("token")
                .AddIdentityServerAuthentication("token", options =>
                {
                    options.Authority = m_authURI.Value;
                    options.RequireHttpsMetadata = false;

                    // enable for MTLS scenarios
                    // options.Authority = Constants.AuthorityMtls;

                    options.ApiName = "api1";
                    options.ApiSecret = "secret";

                    options.JwtBearerEvents = new Microsoft.AspNetCore.Authentication.JwtBearer.JwtBearerEvents
                    {
                        OnTokenValidated = e =>
                        {
                            var jwt = e.SecurityToken as JwtSecurityToken;
                            var type = jwt.Header.Typ;

                            if (!string.Equals(type, "at+jwt", StringComparison.Ordinal))
                            {
                                e.Fail("JWT is not an access token");
                            }

                            return Task.CompletedTask;
                        }
                    };
                })
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