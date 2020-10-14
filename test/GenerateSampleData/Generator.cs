using Common;
using Dodo;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.SharedTest;
using Dodo.LocationResources;
using Dodo.Users;
using Dodo.WorkingGroups;
using Resources;
using Resources.Location;
using Resources.Security;
using SharedTest;
using System;
using System.Threading.Tasks;
using Dodo.Users.Tokens;
using System.Collections.Generic;
using Dodo.Analytics;

namespace GenerateSampleData
{
	/// <summary>
	/// This class will go and generate a bunch of sample information. It will take a good few minutes to generate.
	/// The easiest way to get more/less data is just to add/remove cities below.
	/// Be aware that running this utility will REQUIRE providing a valid MapBox public API key `MapBoxGeocodingService_ApiKey` in your configuration
	/// in order to look up the city lat/longs
	/// </summary>
	public static class Generator
	{
		const string UNIVERSAL_PASS = "@12345667";

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
			Analytics.Clear();
			var sysAdmin = GenerateUser(new UserSchema("Rebellion Tom", "test", UNIVERSAL_PASS, "admin1@web.com"), out var admin1context);
			using (var rscLock = new ResourceLock(sysAdmin))
			{
				sysAdmin.TokenCollection.AddOrUpdate(sysAdmin, new SysadminToken());
				ResourceUtility.GetManager<User>().Update(sysAdmin, rscLock);
			}
			var tasks = new List<Task>();
			foreach (var city in m_cities)
			{
				tasks.Add(Task.Run(() => GenerateRebellion(city, admin1context)));
			}
			Task.WaitAll(tasks.ToArray());
		}

		public static async Task GenerateRebellion(string city, AccessContext context)
		{
			var location = await LocationManager.GetLocation(city);

			var rnd = new Random();
			// Make local groups
			TestBase.CreateNewObject<LocalGroup>(context, new LocalGroupSchema(city, SchemaGenerator.SampleDescription, location));
			TestBase.CreateNewObject<LocalGroup>(context, new LocalGroupSchema($"East {city}", SchemaGenerator.SampleDescription, new GeoLocation(location.Latitude, location.Longitude - .1 * rnd.NextDouble())));
			TestBase.CreateNewObject<LocalGroup>(context, new LocalGroupSchema($"West {city}", SchemaGenerator.SampleDescription, new GeoLocation(location.Latitude, location.Longitude + .1 * rnd.NextDouble())));
			TestBase.CreateNewObject<LocalGroup>(context, new LocalGroupSchema($"North {city}", SchemaGenerator.SampleDescription, new GeoLocation(location.Latitude - .1 * rnd.NextDouble(), location.Longitude)));
			TestBase.CreateNewObject<LocalGroup>(context, new LocalGroupSchema($"South {city}", SchemaGenerator.SampleDescription, new GeoLocation(location.Latitude - .1 * rnd.NextDouble(), location.Longitude)));

			var rootDate = DateTime.Today + TimeSpan.FromDays(rnd.Next(365));
			var currentRebellion = TestBase.CreateNewObject<Rebellion>(context,
				new RebellionSchema($"{city} Rebellion", SchemaGenerator.SampleDescription, location, rootDate - TimeSpan.FromDays(2), rootDate + TimeSpan.FromDays(2)), seed: false);
			var siteOccupation = TestBase.CreateNewObject<Site>(context, new SiteSchema("Central Occupation", currentRebellion.Guid.ToString().ToString(), location, SchemaGenerator.SampleDescription), seed: false);
			var actionOccupation = TestBase.CreateNewObject<Event>(context, new EventSchema("Protest For Nature", currentRebellion.Guid.ToString().ToString(), new GeoLocation(location.ToCoordinate().Latitude + 0.05 * rnd.NextDouble(), location.ToCoordinate().Longitude + 0.05), SchemaGenerator.SampleDescription, currentRebellion.StartDate + TimeSpan.FromDays(1), currentRebellion.StartDate + TimeSpan.FromDays(1) + TimeSpan.FromHours(4)));
			var march = TestBase.CreateNewObject<Event>(context, new EventSchema("Youth March", currentRebellion.Guid.ToString().ToString(), new GeoLocation(location.ToCoordinate().Latitude - 0.05 * rnd.NextDouble(), location.ToCoordinate().Longitude + 0.05), SchemaGenerator.SampleDescription, currentRebellion.StartDate + TimeSpan.FromDays(1), currentRebellion.StartDate + TimeSpan.FromDays(1) + TimeSpan.FromHours(4)));
			var sanctuary = TestBase.CreateNewObject<Site>(context, new SiteSchema("Second Occupation", currentRebellion.Guid.ToString().ToString(), new GeoLocation(location.ToCoordinate().Latitude - 0.05 * rnd.NextDouble(), location.ToCoordinate().Longitude - 0.05), SchemaGenerator.SampleDescription));
			var evt = TestBase.CreateNewObject<Event>(context, new EventSchema("Activism Workshop", currentRebellion.Guid.ToString().ToString(), new GeoLocation(location.ToCoordinate().Latitude - 0.05 * rnd.NextDouble(), location.ToCoordinate().Longitude - 0.05), SchemaGenerator.SampleDescription, currentRebellion.StartDate + TimeSpan.FromDays(1), currentRebellion.StartDate + TimeSpan.FromDays(1) + TimeSpan.FromHours(4)));

			var actionSupport = TestBase.CreateNewObject<WorkingGroup>(context, new WorkingGroupSchema("Action Support",
				SchemaGenerator.SampleDescription, currentRebellion.Guid.ToString().ToString()), seed: false);
			var rebelRiders = TestBase.CreateNewObject<WorkingGroup>(context, new WorkingGroupSchema("Rebel Riders",
				SchemaGenerator.SampleDescription, actionSupport.Guid.ToString().ToString()), seed: false);
			var rebelRiderRole = TestBase.CreateNewObject<Role>(context, new RoleSchema("Rebel Rider Team",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, rebelRiders.Guid.ToString().ToString()), seed: false);

			var deescalation = TestBase.CreateNewObject<WorkingGroup>(context, new WorkingGroupSchema("Non-Violence & De-escalation",
				SchemaGenerator.SampleDescription, actionSupport.Guid.ToString().ToString()), seed: false);

			var firstAid = TestBase.CreateNewObject<WorkingGroup>(context, new WorkingGroupSchema("First Aid",
				SchemaGenerator.SampleDescription, actionSupport.Guid.ToString().ToString()), seed: false);

			var worldBuilding = TestBase.CreateNewObject<WorkingGroup>(context, new WorkingGroupSchema("Worldbuilding & Production",
				SchemaGenerator.SampleDescription, currentRebellion.Guid.ToString()), seed: false);
			var sustenance = TestBase.CreateNewObject<WorkingGroup>(context, new WorkingGroupSchema("Sustenance",
				SchemaGenerator.SampleDescription, worldBuilding.Guid.ToString()), seed: false);
			var kitchenHelper = TestBase.CreateNewObject<Role>(context, new RoleSchema("Kitchen Helper",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, sustenance.Guid.ToString()), seed: false);
			var cartCrew = TestBase.CreateNewObject<Role>(context, new RoleSchema("Food Cart Crew",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, sustenance.Guid.ToString()), seed: false);

			var sitebuilding = TestBase.CreateNewObject<WorkingGroup>(context, new WorkingGroupSchema("Site Building",
				SchemaGenerator.SampleDescription, worldBuilding.Guid.ToString()), seed: false);
			var siteTeam = TestBase.CreateNewObject<Role>(context, new RoleSchema("Installation Team",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, sitebuilding.Guid.ToString()), seed: false);
			var sanitiationTeam = TestBase.CreateNewObject<Role>(context, new RoleSchema("Sanitation Team",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, sitebuilding.Guid.ToString()), seed: false);
			var transportTeam = TestBase.CreateNewObject<Role>(context, new RoleSchema("Transport Team",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, sitebuilding.Guid.ToString()), seed: false);

			var mediaandmessaging = TestBase.CreateNewObject<WorkingGroup>(context, new WorkingGroupSchema("Media & Messaging",
				SchemaGenerator.SampleDescription, currentRebellion.Guid.ToString()), seed: false);
			var spokespeople = TestBase.CreateNewObject<Role>(context, new RoleSchema("Spokespeople & Training",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, mediaandmessaging.Guid.ToString()), seed: false);
			var press = TestBase.CreateNewObject<Role>(context, new RoleSchema("Press",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, mediaandmessaging.Guid.ToString()), seed: false);
			var video = TestBase.CreateNewObject<Role>(context, new RoleSchema("Video",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, mediaandmessaging.Guid.ToString()), seed: false);
			var photography = TestBase.CreateNewObject<Role>(context, new RoleSchema("Photography",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, mediaandmessaging.Guid.ToString()), seed: false);

			var movementSupp = TestBase.CreateNewObject<WorkingGroup>(context, new WorkingGroupSchema("Movement Support",
				SchemaGenerator.SampleDescription, currentRebellion.Guid.ToString()), seed: false);

			var stewarding = TestBase.CreateNewObject<WorkingGroup>(context, new WorkingGroupSchema("Stewards",
				SchemaGenerator.SampleDescription, movementSupp.Guid.ToString()), seed: false);
			var stewTeam = TestBase.CreateNewObject<Role>(context, new RoleSchema("Stewarding Team",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, stewarding.Guid.ToString()), seed: false);

			var outreach = TestBase.CreateNewObject<WorkingGroup>(context, new WorkingGroupSchema("Outreach",
				SchemaGenerator.SampleDescription, movementSupp.Guid.ToString()), seed: false);
			var welcomeTeam = TestBase.CreateNewObject<Role>(context, new RoleSchema("Welcome Team",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, stewarding.Guid.ToString()), seed: false);
			var outreachTeam = TestBase.CreateNewObject<Role>(context, new RoleSchema("Roving Outreach Team",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, stewarding.Guid.ToString()), seed: false);
			var inductions = TestBase.CreateNewObject<Role>(context, new RoleSchema("Inductions & Training",
				SchemaGenerator.SampleDescription, SchemaGenerator.SampleRoleInstructions, stewarding.Guid.ToString()), seed: false);
		}

		private static User GenerateUser(UserSchema schema, out AccessContext context)
		{
			return TestBase.GenerateUser(schema, out context);
		}
	}
}
