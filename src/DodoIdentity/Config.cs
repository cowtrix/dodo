// Copyright (c) Brock Allen & Dominick Baier. All rights reserved.
// Licensed under the Apache License, Version 2.0. See LICENSE in the project root for license information.


using IdentityServer4;
using IdentityServer4.Models;
using System.Collections.Generic;

namespace DodoIdentity
{
	public static class Config
	{
		public static IEnumerable<IdentityResource> Ids =>
			new IdentityResource[]
			{
				new IdentityResource("User", new []
				{
					"Username"
				})
			};


		public static IEnumerable<ApiResource> Apis =>
			new ApiResource[]
			{
				new ApiResource("api", "Resource API")
			};


		public static IEnumerable<Client> Clients =>
			new Client[]
			{
				// SPA client using code flow + pkce
				new Client
				{
					ClientId = "spa",
					ClientName = "SPA Client",
					
					AllowedGrantTypes = GrantTypes.ResourceOwnerPassword,
					RequirePkce = true,
					RequireClientSecret = false,
					AllowedScopes = new List<string>
					{
						"api"
					},
				}
			};
	}
}