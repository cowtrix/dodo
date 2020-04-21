using Common;
using Dodo;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.SharedTest;
using Dodo.Sites;
using Dodo.Users;
using Dodo.WorkingGroups;
using Resources;
using Resources.Location;
using Resources.Security;
using SharedTest;
using System;
using System.Threading.Tasks;

namespace GenerateSampleData
{
	public static class Generator
	{
		const string UNIVERSAL_PASS = "@12345667";
		static UserFactory UserFactory = new UserFactory();
		static RebellionFactory RebellionFactory = new RebellionFactory();
		static WorkingGroupFactory WorkingGroupFactory = new WorkingGroupFactory();
		static LocalGroupFactory LocalGroupFactory = new LocalGroupFactory();
		static RoleFactory RoleFactory = new RoleFactory();
		static SiteFactory SiteFactory = new SiteFactory();

		static string[] m_cities = new[]
		{
			"London", "Paris", "Rome", "Denver", "New York", "Los Angeles", "San Francisco", "Vancouver", "Amsterdam", "Sydney"
		};

		static async Task Main(string[] args)
		{
			Logger.CurrentLogLevel = ELogLevel.Debug;
			await Generate();
		}

		public static async Task Generate()
		{
			ResourceUtility.ClearAllManagers();
			GenerateUser(new UserSchema("Rebellion Tom", "tom", UNIVERSAL_PASS, "admin1@web.com"), out var admin1context);
			foreach(var city in m_cities)
			{
				await GenerateRebellion(city, admin1context);
			}
		}

		public static async Task GenerateRebellion(string city, AccessContext context)
		{
			var location = await LocationManager.GetLocation(city);

			// Make local groups
			LocalGroupFactory.CreateObject(context, new LocalGroupSchema(city, SchemaGenerator.SampleDescription, location));
			LocalGroupFactory.CreateObject(context, new LocalGroupSchema($"East {city}", SchemaGenerator.SampleDescription, new GeoLocation(location.Latitude, location.Longitude - 1)));
			LocalGroupFactory.CreateObject(context, new LocalGroupSchema($"West {city}", SchemaGenerator.SampleDescription, new GeoLocation(location.Latitude, location.Longitude + 1)));
			LocalGroupFactory.CreateObject(context, new LocalGroupSchema($"North {city}", SchemaGenerator.SampleDescription, new GeoLocation(location.Latitude, location.Longitude - 1)));
			LocalGroupFactory.CreateObject(context, new LocalGroupSchema($"South {city}", SchemaGenerator.SampleDescription, new GeoLocation(location.Latitude, location.Longitude - 1)));

			var currentRebellion = RebellionFactory.CreateObject(context,
				new RebellionSchema($"{city} Rebellion", SchemaGenerator.SampleDescription, location, DateTime.Today - TimeSpan.FromDays(2), DateTime.Today + TimeSpan.FromDays(2)));
			var siteOccupation = SiteFactory.CreateObject(context, new SiteSchema("Central Occupation", typeof(PermanentSite).FullName, currentRebellion.Guid, location, SchemaGenerator.SampleDescription));
			var actionOccupation = SiteFactory.CreateObject(context, new SiteSchema("Protest For Nature", typeof(EventSite).FullName, currentRebellion.Guid, new GeoLocation(location.ToCoordinate().Latitude + 0.05, location.ToCoordinate().Longitude + 0.05), SchemaGenerator.SampleDescription));
			var march = SiteFactory.CreateObject(context, new SiteSchema("Youth March", typeof(MarchSite).FullName, currentRebellion.Guid, new GeoLocation(location.ToCoordinate().Latitude - 0.05, location.ToCoordinate().Longitude + 0.05), SchemaGenerator.SampleDescription));
			var sanctuary = SiteFactory.CreateObject(context, new SiteSchema("Second Occupation", typeof(PermanentSite).FullName, currentRebellion.Guid, new GeoLocation(location.ToCoordinate().Latitude - 0.05, location.ToCoordinate().Longitude - 0.05), SchemaGenerator.SampleDescription));
			var evt = SiteFactory.CreateObject(context, new SiteSchema("Activism Workshop", typeof(EventSite).FullName, currentRebellion.Guid, new GeoLocation(location.ToCoordinate().Latitude - 0.05, location.ToCoordinate().Longitude - 0.05), SchemaGenerator.SampleDescription));


			var actionSupport = WorkingGroupFactory.CreateTypedObject(context, new WorkingGroupSchema("Action Support",
				SchemaGenerator.SampleDescription, currentRebellion.Guid));
			var rebelRiders = WorkingGroupFactory.CreateObject(context, new WorkingGroupSchema("Rebel Riders",
				SchemaGenerator.SampleDescription, actionSupport.Guid));
			var rebelRiderRole = RoleFactory.CreateObject(context, new RoleSchema("Rebel Rider Team",
				SchemaGenerator.SampleDescription, rebelRiders.Guid));

			var admin2 = TestBase.GetRandomUser(out _, out var admin2context);
			actionSupport.AddOrUpdateAdmin(context, admin2, admin2context.Passphrase);

			var deescalation = WorkingGroupFactory.CreateObject(context, new WorkingGroupSchema("Non-Violence & De-escalation",
				SchemaGenerator.SampleDescription, actionSupport.Guid));

			var firstAid = WorkingGroupFactory.CreateObject(context, new WorkingGroupSchema("First Aid",
				SchemaGenerator.SampleDescription, actionSupport.Guid));

			var worldBuilding = WorkingGroupFactory.CreateObject(context, new WorkingGroupSchema("Worldbuilding & Production",
				SchemaGenerator.SampleDescription, currentRebellion.Guid));
			var sustenance = WorkingGroupFactory.CreateObject(context, new WorkingGroupSchema("Sustenance",
				SchemaGenerator.SampleDescription, worldBuilding.Guid));
			var kitchenHelper = RoleFactory.CreateObject(context, new RoleSchema("Kitchen Helper",
				SchemaGenerator.SampleDescription, sustenance.Guid));
			var cartCrew = RoleFactory.CreateObject(context, new RoleSchema("Food Cart Crew",
				SchemaGenerator.SampleDescription, sustenance.Guid));

			var sitebuilding = WorkingGroupFactory.CreateObject(context, new WorkingGroupSchema("Site Building",
				SchemaGenerator.SampleDescription, worldBuilding.Guid));
			var siteTeam = RoleFactory.CreateObject(context, new RoleSchema("Installation Team",
				SchemaGenerator.SampleDescription, sitebuilding.Guid));
			var sanitiationTeam = RoleFactory.CreateObject(context, new RoleSchema("Sanitation Team",
				SchemaGenerator.SampleDescription, sitebuilding.Guid));
			var transportTeam = RoleFactory.CreateObject(context, new RoleSchema("Transport Team",
				SchemaGenerator.SampleDescription, sitebuilding.Guid));

			var mediaandmessaging = WorkingGroupFactory.CreateObject(context, new WorkingGroupSchema("Media & Messaging",
				SchemaGenerator.SampleDescription, currentRebellion.Guid));
			var spokespeople = RoleFactory.CreateObject(context, new RoleSchema("Spokespeople & Training",
				SchemaGenerator.SampleDescription, mediaandmessaging.Guid));
			var press = RoleFactory.CreateObject(context, new RoleSchema("Press",
				SchemaGenerator.SampleDescription, mediaandmessaging.Guid));
			var video = RoleFactory.CreateObject(context, new RoleSchema("Video",
				SchemaGenerator.SampleDescription, mediaandmessaging.Guid));
			var photography = RoleFactory.CreateObject(context, new RoleSchema("Photography",
				SchemaGenerator.SampleDescription, mediaandmessaging.Guid));

			var movementSupp = WorkingGroupFactory.CreateObject(context, new WorkingGroupSchema("Movement Support",
				SchemaGenerator.SampleDescription, currentRebellion.Guid));

			var stewarding = WorkingGroupFactory.CreateObject(context, new WorkingGroupSchema("Stewards",
				SchemaGenerator.SampleDescription, movementSupp.Guid));
			var stewTeam = RoleFactory.CreateObject(context, new RoleSchema("Stewarding Team",
				SchemaGenerator.SampleDescription, stewarding.Guid));

			var outreach = WorkingGroupFactory.CreateObject(context, new WorkingGroupSchema("Outreach",
				SchemaGenerator.SampleDescription, movementSupp.Guid));
			var welcomeTeam = RoleFactory.CreateObject(context, new RoleSchema("Welcome Team",
				SchemaGenerator.SampleDescription, stewarding.Guid));
			var outreachTeam = RoleFactory.CreateObject(context, new RoleSchema("Roving Outreach Team",
				SchemaGenerator.SampleDescription, stewarding.Guid));
			var inductions = RoleFactory.CreateObject(context, new RoleSchema("Inductions & Training",
				SchemaGenerator.SampleDescription, stewarding.Guid));
		}

		private static User GenerateUser(UserSchema schema, out AccessContext context)
		{
			var user = UserFactory.CreateObject(default(AccessContext), schema);
			context = new AccessContext(user, new Passphrase(user.AuthData.PassPhrase.GetValue(new Passphrase(schema.Password))));
			return user;
		}
	}
}
