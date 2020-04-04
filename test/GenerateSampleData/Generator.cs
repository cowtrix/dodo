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
using Resources.Security;
using System;

namespace GenerateSampleData
{
	class Generator
	{
		const string UNIVERSAL_PASS = "@12345667";
		static UserFactory UserFactory = new UserFactory();
		static RebellionFactory RebellionFactory = new RebellionFactory();
		static WorkingGroupFactory WorkingGroupFactory = new WorkingGroupFactory();
		static LocalGroupFactory LocalGroupFactory = new LocalGroupFactory();
		static RoleFactory RoleFactory = new RoleFactory();
		static SiteFactory SiteFactory = new SiteFactory();

		static void Main(string[] args)
		{
			Logger.CurrentLogLevel = ELogLevel.Debug;
			ResourceUtility.ClearAllManagers();
			var admin1 = GenerateUser(new UserSchema("Rebellion Tom", "tom", UNIVERSAL_PASS, "admin1@web.com"), out var admin1context);

			var localGroup1 = LocalGroupFactory.CreateObject(admin1context, new LocalGroupSchema("East Random place", SchemaGenerator.SampleMarkdown, SchemaGenerator.RandomLocation));
			var localGroup2 = LocalGroupFactory.CreateObject(admin1context, new LocalGroupSchema("Random City", SchemaGenerator.SampleMarkdown, SchemaGenerator.RandomLocation));


			var amstLocation = new GeoLocation(52.373455, 4.898259);
			var currentRebellion = RebellionFactory.CreateObject(admin1context,
				new RebellionSchema("Amsterdam Rebellion", SchemaGenerator.SampleMarkdown, amstLocation, DateTime.Today - TimeSpan.FromDays(2), DateTime.Today + TimeSpan.FromDays(2)));
			var siteOccupation = SiteFactory.CreateObject(admin1context, new SiteSchema("Occupational Site", typeof(PermanentSite).FullName, currentRebellion.Guid, amstLocation, SchemaGenerator.SampleMarkdown));
			var actionOccupation = SiteFactory.CreateObject(admin1context, new SiteSchema("Action", typeof(EventSite).FullName, currentRebellion.Guid, new GeoLocation(amstLocation.ToCoordinate().Latitude + 0.05, amstLocation.ToCoordinate().Longitude + 0.05), SchemaGenerator.SampleMarkdown));
			var march = SiteFactory.CreateObject(admin1context, new SiteSchema("March", typeof(MarchSite).FullName, currentRebellion.Guid, new GeoLocation(amstLocation.ToCoordinate().Latitude - 0.05, amstLocation.ToCoordinate().Longitude + 0.05), SchemaGenerator.SampleMarkdown));
			var sanctuary = SiteFactory.CreateObject(admin1context, new SiteSchema("Sanctuary Site", typeof(PermanentSite).FullName, currentRebellion.Guid, new GeoLocation(amstLocation.ToCoordinate().Latitude - 0.05, amstLocation.ToCoordinate().Longitude - 0.05), SchemaGenerator.SampleMarkdown));
			var evt = SiteFactory.CreateObject(admin1context, new SiteSchema("Event Site", typeof(EventSite).FullName, currentRebellion.Guid, new GeoLocation(amstLocation.ToCoordinate().Latitude - 0.05, amstLocation.ToCoordinate().Longitude - 0.05), SchemaGenerator.SampleMarkdown));


			var actionSupport = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Action Support",
				SchemaGenerator.SampleMarkdown, currentRebellion.Guid));
			var rebelRiders = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Rebel Riders",
				SchemaGenerator.SampleMarkdown, actionSupport.Guid));
			var rebelRiderRole = RoleFactory.CreateObject(admin1context, new RoleSchema("Rebel Rider Team",
				SchemaGenerator.SampleMarkdown, rebelRiders.Guid));

			var admin2 = GenerateUser(new UserSchema("Action Support Dave", "dave", UNIVERSAL_PASS, "admin2@web.com"), out var admin2context);
			actionSupport.AddOrUpdateAdmin(admin1context, admin2, admin2context.Passphrase);

			var deescalation = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Non-Violence & De-escalation",
				SchemaGenerator.SampleMarkdown, actionSupport.Guid));

			var firstAid = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("First Aid",
				SchemaGenerator.SampleMarkdown, actionSupport.Guid));

			var worldBuilding = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Worldbuilding & Production",
				SchemaGenerator.SampleMarkdown, currentRebellion.Guid));
			var sustenance = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Sustenance",
				SchemaGenerator.SampleMarkdown, worldBuilding.Guid));
			var kitchenHelper = RoleFactory.CreateObject(admin1context, new RoleSchema("Kitchen Helper",
				SchemaGenerator.SampleMarkdown, sustenance.Guid));
			var cartCrew = RoleFactory.CreateObject(admin1context, new RoleSchema("Food Cart Crew",
				SchemaGenerator.SampleMarkdown, sustenance.Guid));

			var sitebuilding = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Site Building",
				SchemaGenerator.SampleMarkdown, worldBuilding.Guid));
			var siteTeam = RoleFactory.CreateObject(admin1context, new RoleSchema("Installation Team",
				SchemaGenerator.SampleMarkdown, sitebuilding.Guid));
			var sanitiationTeam = RoleFactory.CreateObject(admin1context, new RoleSchema("Sanitation Team",
				SchemaGenerator.SampleMarkdown, sitebuilding.Guid));
			var transportTeam = RoleFactory.CreateObject(admin1context, new RoleSchema("Transport Team",
				SchemaGenerator.SampleMarkdown, sitebuilding.Guid));

			var mediaandmessaging = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Media & Messaging",
				SchemaGenerator.SampleMarkdown, currentRebellion.Guid));
			var spokespeople = RoleFactory.CreateObject(admin1context, new RoleSchema("Spokespeople & Training",
				SchemaGenerator.SampleMarkdown, mediaandmessaging.Guid));
			var press = RoleFactory.CreateObject(admin1context, new RoleSchema("Press",
				SchemaGenerator.SampleMarkdown, mediaandmessaging.Guid));
			var video = RoleFactory.CreateObject(admin1context, new RoleSchema("Video",
				SchemaGenerator.SampleMarkdown, mediaandmessaging.Guid));
			var photography = RoleFactory.CreateObject(admin1context, new RoleSchema("Photography",
				SchemaGenerator.SampleMarkdown, mediaandmessaging.Guid));

			var movementSupp = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Movement Support",
				SchemaGenerator.SampleMarkdown, currentRebellion.Guid));

			var stewarding = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Stewards",
				SchemaGenerator.SampleMarkdown, movementSupp.Guid));
			var stewTeam = RoleFactory.CreateObject(admin1context, new RoleSchema("Stewarding Team",
				SchemaGenerator.SampleMarkdown, stewarding.Guid));

			var outreach = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Outreach",
				SchemaGenerator.SampleMarkdown, movementSupp.Guid));
			var welcomeTeam = RoleFactory.CreateObject(admin1context, new RoleSchema("Welcome Team",
				SchemaGenerator.SampleMarkdown, stewarding.Guid));
			var outreachTeam = RoleFactory.CreateObject(admin1context, new RoleSchema("Roving Outreach Team",
				SchemaGenerator.SampleMarkdown, stewarding.Guid));
			var inductions = RoleFactory.CreateObject(admin1context, new RoleSchema("Inductions & Training",
				SchemaGenerator.SampleMarkdown, stewarding.Guid));
		}

		private static User GenerateUser(UserSchema schema, out AccessContext context)
		{
			var user = UserFactory.CreateObject(default(AccessContext), schema);
			context = new AccessContext(user, new Passphrase(user.AuthData.PassPhrase.GetValue(new Passphrase(schema.Password))));
			return user;
		}
	}
}
