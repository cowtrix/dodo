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
			ResourceUtility.ClearAllManagers();
			var admin1 = GenerateUser(new UserSchema("Rebellion Tom", "tom", UNIVERSAL_PASS, "admin1@web.com"), out var admin1context);

			var localGroup1 = LocalGroupFactory.CreateObject(admin1context, new LocalGroupSchema("East Randomplace", SchemaGenerator.SampleMarkdown, SchemaGenerator.RandomLocation));
			var localGroup2 = LocalGroupFactory.CreateObject(admin1context, new LocalGroupSchema("Random City", SchemaGenerator.SampleMarkdown, SchemaGenerator.RandomLocation));


			var amstLocation = new GeoLocation(52.373455, 4.898259);
			var currentRebellion = RebellionFactory.CreateObject(admin1context, 
				new RebellionSchema("Amsterdam Rebllion", SchemaGenerator.SampleMarkdown, amstLocation, DateTime.Today - TimeSpan.FromDays(2), DateTime.Today + TimeSpan.FromDays(2)));
			var siteOccupation = SiteFactory.CreateObject(admin1context, new SiteSchema("Occupationsal Site", typeof(OccupationalSite).FullName, currentRebellion.GUID, amstLocation, SchemaGenerator.SampleMarkdown));
			var actionOccupation = SiteFactory.CreateObject(admin1context, new SiteSchema("Action Site", typeof(ActionSite).FullName, currentRebellion.GUID, new GeoLocation(amstLocation.Coordinate.Latitude + 0.05, amstLocation.Coordinate.Longitude + 0.05), SchemaGenerator.SampleMarkdown));
			var march = SiteFactory.CreateObject(admin1context, new SiteSchema("March", typeof(March).FullName, currentRebellion.GUID, new GeoLocation(amstLocation.Coordinate.Latitude - 0.05, amstLocation.Coordinate.Longitude + 0.05), SchemaGenerator.SampleMarkdown));
			var sanctuary = SiteFactory.CreateObject(admin1context, new SiteSchema("Sanctuary Site", typeof(Sanctuary).FullName, currentRebellion.GUID, new GeoLocation(amstLocation.Coordinate.Latitude - 0.05, amstLocation.Coordinate.Longitude - 0.05), SchemaGenerator.SampleMarkdown));
			
			var actionSupport = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Action Support",
				SchemaGenerator.SampleMarkdown, currentRebellion.GUID));
			var rebelRiders = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Rebel Riders",
				SchemaGenerator.SampleMarkdown, actionSupport.GUID));
			var rebelRiderRole = RoleFactory.CreateObject(admin1context, new RoleSchema( "Rebel Rider Team",
				SchemaGenerator.SampleMarkdown, rebelRiders.GUID));

			var admin2 = GenerateUser(new UserSchema("Action Support Dave", "dave", UNIVERSAL_PASS, "admin2@web.com"), out var admin2context);
			actionSupport.AddOrUpdateAdmin(admin1context, admin2, admin2context.Passphrase);

			var deescalation = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Non-Violence & De-escalation",
				SchemaGenerator.SampleMarkdown, actionSupport.GUID));

			var firstAid = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("First Aid",
				SchemaGenerator.SampleMarkdown, actionSupport.GUID));

			var worldBuilding = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Worldbuilding & Production",
				SchemaGenerator.SampleMarkdown, currentRebellion.GUID));
			var sustenance = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Sustenance",
				SchemaGenerator.SampleMarkdown, worldBuilding.GUID));
			var kitchenHelper = RoleFactory.CreateObject(admin1context, new RoleSchema( "Kitchen Helper",
				SchemaGenerator.SampleMarkdown, sustenance.GUID));
			var cartCrew = RoleFactory.CreateObject(admin1context, new RoleSchema( "Food Cart Crew",
				SchemaGenerator.SampleMarkdown, sustenance.GUID));

			var sitebuilding = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Site Building",
				SchemaGenerator.SampleMarkdown, worldBuilding.GUID));
			var siteTeam = RoleFactory.CreateObject(admin1context, new RoleSchema( "Installation Team",
				SchemaGenerator.SampleMarkdown, sitebuilding.GUID));
			var sanitiationTeam = RoleFactory.CreateObject(admin1context, new RoleSchema( "Sanitation Team",
				SchemaGenerator.SampleMarkdown, sitebuilding.GUID));
			var transportTeam = RoleFactory.CreateObject(admin1context, new RoleSchema( "Transport Team",
				SchemaGenerator.SampleMarkdown, sitebuilding.GUID));

			var mediaandmessaging = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Media & Messaging",
				SchemaGenerator.SampleMarkdown, currentRebellion.GUID));
			var spokespeople = RoleFactory.CreateObject(admin1context, new RoleSchema( "Spokespeople & Training",
				SchemaGenerator.SampleMarkdown, mediaandmessaging.GUID));
			var press = RoleFactory.CreateObject(admin1context, new RoleSchema( "Press",
				SchemaGenerator.SampleMarkdown, mediaandmessaging.GUID));
			var video = RoleFactory.CreateObject(admin1context, new RoleSchema( "Video",
				SchemaGenerator.SampleMarkdown, mediaandmessaging.GUID));
			var photography = RoleFactory.CreateObject(admin1context, new RoleSchema( "Photography",
				SchemaGenerator.SampleMarkdown, mediaandmessaging.GUID));

			var movementSupp = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Movement Support",
				SchemaGenerator.SampleMarkdown, currentRebellion.GUID));

			var stewarding = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Stewards",
				SchemaGenerator.SampleMarkdown, movementSupp.GUID));
			var stewTeam = RoleFactory.CreateObject(admin1context, new RoleSchema( "Stewarding Team",
				SchemaGenerator.SampleMarkdown, stewarding.GUID));

			var outreach = WorkingGroupFactory.CreateObject(admin1context, new WorkingGroupSchema("Outreach",
				SchemaGenerator.SampleMarkdown, movementSupp.GUID));
			var welcomeTeam = RoleFactory.CreateObject(admin1context, new RoleSchema( "Welcome Team",
				SchemaGenerator.SampleMarkdown, stewarding.GUID));
			var outreachTeam = RoleFactory.CreateObject(admin1context, new RoleSchema( "Roving Outreach Team",
				SchemaGenerator.SampleMarkdown, stewarding.GUID));
			var inductions = RoleFactory.CreateObject(admin1context, new RoleSchema( "Inductions & Training",
				SchemaGenerator.SampleMarkdown, stewarding.GUID));
		}
		
		private static User GenerateUser(UserSchema schema, out AccessContext context)
		{
			var user = UserFactory.CreateObject(default(AccessContext), schema);
			context = new AccessContext(user, new Passphrase(user.AuthData.PassPhrase.GetValue(new Passphrase(schema.Password))));
			return user;
		}
	}
}
