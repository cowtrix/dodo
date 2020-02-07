﻿namespace Microsoft.AspNetCore.Identity.MongoDB
{
	using global::MongoDB.Driver;

	public static class IndexChecks
	{
		public static void EnsureUniqueIndexOnNormalizedUserName<TUser>(IMongoCollection<TUser> users)
			where TUser : Dodo.Users.User
		{
			var userName = Builders<TUser>.IndexKeys.Ascending(t => t.AuthData.Username);
			var unique = new CreateIndexOptions {Unique = true};
			users.Indexes.CreateOneAsync(userName, unique);
		}

		public static void EnsureUniqueIndexOnNormalizedRoleName<TRole>(IMongoCollection<TRole> roles)
			where TRole : IdentityRole
		{
			var roleName = Builders<TRole>.IndexKeys.Ascending(t => t.NormalizedName);
			var unique = new CreateIndexOptions {Unique = true};
			roles.Indexes.CreateOneAsync(roleName, unique);
		}

		public static void EnsureUniqueIndexOnNormalizedEmail<TUser>(IMongoCollection<TUser> users)
			where TUser : Dodo.Users.User
		{
			var email = Builders<TUser>.IndexKeys.Ascending(t => t.PersonalData);
			var unique = new CreateIndexOptions {Unique = true};
			users.Indexes.CreateOneAsync(email, unique);
		}

		/// <summary>
		///     ASP.NET Core Identity now searches on normalized fields so these indexes are no longer required, replace with
		///     normalized checks.
		/// </summary>
		public static class OptionalIndexChecks
		{
			public static void EnsureUniqueIndexOnUserName<TUser>(IMongoCollection<TUser> users)
				where TUser : Dodo.Users.User
			{
				var userName = Builders<TUser>.IndexKeys.Ascending(t => t.AuthData.Username);
				var unique = new CreateIndexOptions {Unique = true};
				users.Indexes.CreateOneAsync(userName, unique);
			}

			public static void EnsureUniqueIndexOnRoleName<TRole>(IMongoCollection<TRole> roles)
				where TRole : IdentityRole
			{
				var roleName = Builders<TRole>.IndexKeys.Ascending(t => t.Name);
				var unique = new CreateIndexOptions {Unique = true};
				roles.Indexes.CreateOneAsync(roleName, unique);
			}

			public static void EnsureUniqueIndexOnEmail<TUser>(IMongoCollection<TUser> users)
				where TUser : Dodo.Users.User
			{
				var email = Builders<TUser>.IndexKeys.Ascending(t => t.PersonalData.Email);
				var unique = new CreateIndexOptions {Unique = true};
				users.Indexes.CreateOneAsync(email, unique);
			}
		}
	}
}