using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace DodoServer
{
	public static class Config
	{
		public static IEnumerable<IdentityResource> Ids =>
			new List<IdentityResource>
			{
				new IdentityResources.OpenId(),
				new IdentityResources.Profile(),
			};


		public static IEnumerable<ApiResource> Apis =>
			new List<ApiResource>
			{
				new ApiResource("api1", "My API")
			};

		public static IEnumerable<Client> Clients =>
			new List<Client>
			{
				// interactive ASP.NET Core MVC client
				new Client
				{
					ClientId = "spa",
					RequireClientSecret = false,
					//ClientSecrets = { new Secret("secret".Sha256()) },

					//AllowedGrantTypes = GrantTypes.Code,
					
					RequireConsent = false,
					RequirePkce = true,
				
					// where to redirect to after login
					RedirectUris = { DodoServer.HttpsUrl },

					// where to redirect to after logout
					PostLogoutRedirectUris = { $"{DodoServer.HttpsUrl}/api"},

					/*AllowedScopes = new List<string>
					{
						IdentityServerConstants.StandardScopes.OpenId,
						IdentityServerConstants.StandardScopes.Profile,
						"api1"
					},*/

					AllowOfflineAccess = true
				}
			};
	}
}
