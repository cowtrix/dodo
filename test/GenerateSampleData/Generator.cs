using Common;
using Dodo;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.SharedTest;
using Dodo.Users;
using Dodo.WorkingGroups;
using REST;
using REST.Security;
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

		static void Main(string[] args)
		{
			ResourceUtility.ClearAllManagers();
			var admin1 = GenerateUser(new UserSchema(default, "Rebellion Tom", "tom", UNIVERSAL_PASS, "admin1@web.com"), out var admin1context);
			var currentRebellion = RebellionFactory.CreateObject(new RebellionSchema(admin1context, "Amsterdam Rebllion", 
				new GeoLocation(52.373455, 4.898259), SchemaGenerator.SampleMarkdown, null, 
				DateTime.Today - TimeSpan.FromDays(2), DateTime.Today + TimeSpan.FromDays(2)));

			var actionSupport = WorkingGroupFactory.CreateObject(new WorkingGroupSchema(admin1context, "Action Support",
				SchemaGenerator.SampleMarkdown, currentRebellion));
			var rebelRiders = WorkingGroupFactory.CreateObject(new WorkingGroupSchema(admin1context, "Rebel Riders",
				SchemaGenerator.SampleMarkdown, actionSupport));
			var rebelRiderRole = RoleFactory.CreateObject(new RoleSchema(admin1context, "Rebel Rider Team",
				SchemaGenerator.SampleMarkdown, rebelRiders));

			var admin2 = GenerateUser(new UserSchema(default, "Action Support Dave", "dave", UNIVERSAL_PASS, "admin2@web.com"), out var admin2context);
			actionSupport.AddAdmin(admin1context, admin2, admin2context.Passphrase);

			var deescalation = WorkingGroupFactory.CreateObject(new WorkingGroupSchema(admin1context, "Non-Violence & De-escalation",
				SchemaGenerator.SampleMarkdown, actionSupport));

			var firstAid = WorkingGroupFactory.CreateObject(new WorkingGroupSchema(admin1context, "First Aid",
				SchemaGenerator.SampleMarkdown, actionSupport));

			var worldBuilding = WorkingGroupFactory.CreateObject(new WorkingGroupSchema(admin1context, "Worldbuilding & Production",
				SchemaGenerator.SampleMarkdown, currentRebellion));
			var sustenance = WorkingGroupFactory.CreateObject(new WorkingGroupSchema(admin1context, "Sustenance",
				SchemaGenerator.SampleMarkdown, worldBuilding));
			var kitchenHelper = RoleFactory.CreateObject(new RoleSchema(admin1context, "Kitchen Helper",
				SchemaGenerator.SampleMarkdown, sustenance));
			var cartCrew = RoleFactory.CreateObject(new RoleSchema(admin1context, "Food Cart Crew",
				SchemaGenerator.SampleMarkdown, sustenance));

			var sitebuilding = WorkingGroupFactory.CreateObject(new WorkingGroupSchema(admin1context, "Site Building",
				SchemaGenerator.SampleMarkdown, worldBuilding));
			var siteTeam = RoleFactory.CreateObject(new RoleSchema(admin1context, "Installation Team",
				SchemaGenerator.SampleMarkdown, sitebuilding));
			var sanitiationTeam = RoleFactory.CreateObject(new RoleSchema(admin1context, "Sanitation Team",
				SchemaGenerator.SampleMarkdown, sitebuilding));
			var transportTeam = RoleFactory.CreateObject(new RoleSchema(admin1context, "Transport Team",
				SchemaGenerator.SampleMarkdown, sitebuilding));

			var mediaandmessaging = WorkingGroupFactory.CreateObject(new WorkingGroupSchema(admin1context, "Media & Messaging",
				SchemaGenerator.SampleMarkdown, currentRebellion));
			var spokespeople = RoleFactory.CreateObject(new RoleSchema(admin1context, "Spokespeople & Training",
				SchemaGenerator.SampleMarkdown, mediaandmessaging));
			var press = RoleFactory.CreateObject(new RoleSchema(admin1context, "Press",
				SchemaGenerator.SampleMarkdown, mediaandmessaging));
			var video = RoleFactory.CreateObject(new RoleSchema(admin1context, "Video",
				SchemaGenerator.SampleMarkdown, mediaandmessaging));
			var photography = RoleFactory.CreateObject(new RoleSchema(admin1context, "Photography",
				SchemaGenerator.SampleMarkdown, mediaandmessaging));

			var movementSupp = WorkingGroupFactory.CreateObject(new WorkingGroupSchema(admin1context, "Movement Support",
				SchemaGenerator.SampleMarkdown, currentRebellion));

			var stewarding = WorkingGroupFactory.CreateObject(new WorkingGroupSchema(admin1context, "Stewards",
				SchemaGenerator.SampleMarkdown, movementSupp));
			var stewTeam = RoleFactory.CreateObject(new RoleSchema(admin1context, "Stewarding Team",
				SchemaGenerator.SampleMarkdown, stewarding));

			var outreach = WorkingGroupFactory.CreateObject(new WorkingGroupSchema(admin1context, "Outreach",
				SchemaGenerator.SampleMarkdown, movementSupp));
			var welcomeTeam = RoleFactory.CreateObject(new RoleSchema(admin1context, "Welcome Team",
				SchemaGenerator.SampleMarkdown, stewarding));
			var outreachTeam = RoleFactory.CreateObject(new RoleSchema(admin1context, "Roving Outreach Team",
				SchemaGenerator.SampleMarkdown, stewarding));
			var inductions = RoleFactory.CreateObject(new RoleSchema(admin1context, "Inductions & Training",
				SchemaGenerator.SampleMarkdown, stewarding));
		}
		
		private static User GenerateUser(UserSchema schema, out AccessContext context)
		{
			var user = UserFactory.CreateObject(schema);
			context = new AccessContext(user, new Passphrase(schema.Password));
			return user;
		}
	}
}
