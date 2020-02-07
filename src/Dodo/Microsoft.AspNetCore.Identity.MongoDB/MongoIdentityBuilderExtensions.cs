// ReSharper disable once CheckNamespace - Common convention to locate extensions in Microsoft namespaces for simplifying autocompletion as a consumer.

namespace Microsoft.Extensions.DependencyInjection
{
	using System;
	using AspNetCore.Identity;
	using AspNetCore.Identity.MongoDB;
    using Dodo.Users;
    using MongoDB.Driver;
    using REST;

    public static class MongoIdentityBuilderExtensions
	{
		/// <summary>
		///     This method only registers mongo stores, you also need to call AddIdentity.
		///     Consider using AddIdentityWithMongoStores.
		/// </summary>
		/// <param name="builder"></param>
		/// <param name="connectionString">Must contain the database name</param>
		public static IdentityBuilder RegisterMongoStores<TRole>(this IdentityBuilder builder, string databaseName)
			where TRole : IdentityRole
		{
			var database = ResourceUtility.MongoDB.GetDatabase(databaseName);
			return builder.RegisterMongoStores(
				p => database.GetCollection<User>("users"),
				p => database.GetCollection<TRole>("roles"));
		}

		/// <summary>
		///     If you want control over creating the users and roles collections, use this overload.
		///     This method only registers mongo stores, you also need to call AddIdentity.
		/// </summary>
		/// <typeparam name="User"></typeparam>
		/// <typeparam name="TRole"></typeparam>
		/// <param name="builder"></param>
		/// <param name="usersCollectionFactory"></param>
		/// <param name="rolesCollectionFactory"></param>
		public static IdentityBuilder RegisterMongoStores<TRole>(this IdentityBuilder builder,
			Func<IServiceProvider, IMongoCollection<User>> usersCollectionFactory,
			Func<IServiceProvider, IMongoCollection<TRole>> rolesCollectionFactory)
			where TRole : IdentityRole
		{
			if (typeof(User) != builder.UserType)
			{
				var message = "User type passed to RegisterMongoStores must match user type passed to AddIdentity. "
				              + $"You passed {builder.UserType} to AddIdentity and {typeof(User)} to RegisterMongoStores, "
				              + "these do not match.";
				throw new ArgumentException(message);
			}
			if (typeof(TRole) != builder.RoleType)
			{
				var message = "Role type passed to RegisterMongoStores must match role type passed to AddIdentity. "
				              + $"You passed {builder.RoleType} to AddIdentity and {typeof(TRole)} to RegisterMongoStores, "
				              + "these do not match.";
				throw new ArgumentException(message);
			}
			builder.Services.AddSingleton<IUserStore<User>>(p => new UserStore());
			builder.Services.AddSingleton<IRoleStore<TRole>>(p => new RoleStore<TRole>(rolesCollectionFactory(p)));
			return builder;
		}

		/// <summary>
		///     This method registers identity services and MongoDB stores using the IdentityUser and IdentityRole types.
		/// </summary>
		/// <param name="services"></param>
		/// <param name="databaseName">Connection string must contain the database name</param>
		public static IdentityBuilder AddIdentityWithMongoStores(this IServiceCollection services, string databaseName)
		{
			return services.AddIdentityWithMongoStoresUsingCustomTypes<IdentityRole>(databaseName);
		}

		/// <summary>
		///     This method allows you to customize the user and role type when registering identity services
		///     and MongoDB stores.
		/// </summary>
		/// <typeparam name="User"></typeparam>
		/// <typeparam name="TRole"></typeparam>
		/// <param name="services"></param>
		/// <param name="databaseName">Connection string must contain the database name</param>
		public static IdentityBuilder AddIdentityWithMongoStoresUsingCustomTypes<TRole>(this IServiceCollection services, string databaseName)
			where TRole : IdentityRole
		{
			return services.AddIdentity<User, TRole>()
				.RegisterMongoStores<TRole>(databaseName);
		}
	}
}