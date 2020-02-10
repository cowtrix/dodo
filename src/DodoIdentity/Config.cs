// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4.Models;
using System.Collections.Generic;

namespace DodoIdentity
{
	public static class Config
	{
		public static IEnumerable<IdentityResource> Ids =>
			new IdentityResource[]
			{
				new IdentityResources.Profile(),
			};


		public static IEnumerable<ApiResource> Apis =>
			new ApiResource[]
			{
				new ApiResource("api1", "My API #1")
			};


		public static IEnumerable<Client> Clients =>
			new Client[]
			{
				// client credentials flow client
				new Client
				{
					ClientId = "client",
					ClientName = "Client Credentials Client",

					AllowedGrantTypes = GrantTypes.ClientCredentials,
					ClientSecrets = { new Secret("511536EF-F270-4058-80CA-1C89C192F69A".Sha256()) },

					AllowedScopes = { "api" },
				},

				// SPA client using code flow + pkce
				new Client
				{
					ClientId = "spa",
					ClientName = "SPA Client",
					ClientUri = DodoIdentity.Program.GetHostname(),

					AllowedGrantTypes = GrantTypes.Code,
					RequirePkce = true,
					RequireClientSecret = false,

					PostLogoutRedirectUris = { "http://localhost:5002/index.html" },
					AllowedCorsOrigins = { "http://localhost:5002" },

					AllowedScopes = { "profile", "api" }
				}
			};
	}
}