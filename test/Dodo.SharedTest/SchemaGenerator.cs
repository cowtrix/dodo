using Common;
using Common.Extensions;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.LocationResources;
using Dodo.Users;
using Dodo.WorkingGroups;
using Resources;
using Resources.Location;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.IO;
using System.Text;

namespace Dodo.SharedTest
{
	public static class SchemaGenerator
	{
		public static ResourceSchemaBase GetRandomSchema<T>(AccessContext context) where T : IRESTResource
		{
			return GetRandomSchema(typeof(T), context);
		}

		public static ResourceSchemaBase GetRandomSchema(Type type, AccessContext context)
		{
			if (type == null)
			{
				throw new ArgumentNullException(nameof(type));
			}
			if (m_mappings.TryGetValue(type, out var schemaFunc))
			{
				return schemaFunc.Invoke(context);
			}
			if (type.BaseType != null)
			{
				return GetRandomSchema(type.BaseType, context);
			}
			throw new Exception($"No schema generator found for type {type}");
		}

		private static Random m_random = new Random();
		public static string SampleDescription => "We are unprepared for the danger our future holds. We face floods, wildfires, extreme weather, crop failure, mass displacement and the breakdown of society. The time for denial is over. It is time to act.\n\nConventional approaches of voting, lobbying, petitions and protest have failed because powerful political and economic interests prevent change. Our strategy is therefore one of non-violent, disruptive civil disobedience – a rebellion.\n\nHistorical evidence shows that we need the involvement of 3.5% of the population to succeed – in the UK that’s about 2 million people.\nHelp XR mobilise and donate [here](https://rebellion.earth/donate/\nExtinction)\n\n[Rebellion](https://rebellion.earth/)\n\n[International](https://rebellion.global/)\n\n1. #TellTheTruth \n\n2. #ActNow \n\n3. #BeyondPolitics";
		public static GeoLocation RandomLocation => new GeoLocation(m_random.NextDouble() * 90, m_random.NextDouble() * 90);
		public static DateTime RandomDate => DateTime.Now + TimeSpan.FromDays(m_random.NextDouble() * 365);
		public static string RandomVideoURL => "https://www.youtube.com/embed/d4QDM_Isi24";

		public static string SampleRoleInstructions { get; set; }

		private static string RandomName<T>() => m_nameGenerators[typeof(T)].Invoke();

		private static Dictionary<Type, Func<AccessContext, ResourceSchemaBase>> m_mappings =
			new Dictionary<Type, Func<AccessContext, ResourceSchemaBase>>()
		{
			{ typeof(User), GetRandomUser },
			{ typeof(Rebellion), GetRandomRebellion },
			{ typeof(WorkingGroup), wg => GetRandomWorkinGroup(wg) },
			{ typeof(LocalGroup), lg => GetRandomLocalGroup(lg) },
			{ typeof(Role), r => GetRandomRole(r) },
			{ typeof(Event), s => GetRandomEvent(s) },
			{ typeof(Site), s => GetRandomSite(s) },
			{ typeof(LocationResourceBase), s => GetRandomSite(s) },
		};


		static string[] m_peopleStrings = new[] { "Dave", "Smith", "John", "Todd", "Jane", "Mary", "Bob", "Suzie", "Raj", "Evan", "Ankara", "Boris", "Roger", "Davies", "White" };
		static string[] m_eventStrings = new[] { "March For Justice", "Protest Against Extinction", "Stop The Destruction Of Nature", "Youth March", "Lessons in Activism Workshop", "NVDA Training",
			"Know Your Rights Training", "Sign-Making Workshop", "Kid's Climate Lesson" };
		static string[] m_siteStrings = new[] { "Central Occupation", "Government Camp", "Burning Earth", "Youth Camp", "Occupation Site", "Central Hub",
			"Secondary Occupation", "Campsite", "Sanctuary Space" };
		static string[] m_roleStrings = new[] { "External Coordinator", "Internal Coordinator", "Organiser", "Driver", "Trainer", "Assistant",
			"General Team Member", "Rebellion Link", "Integration Coordinator" };
		static string[] m_wgStrings = new[] { "Rebel Riders", "Sustenance", "Police Liason", "Media and Messaging", "Press", "Photos",
			"Reactive", "Action Support", "Action Design" };
		static string[] m_lgStrings = new[] { "Test Local Group" };
		static string[] m_rebellionStrings = new[] { "Summer Rebellion", "Highway Rebellion", "Week of Rage", "Fortnight of Disruption", "Wave of Rebellion", "Summer Uprising" };

		private static Dictionary<Type, Func<string>> m_nameGenerators =
			new Dictionary<Type, Func<string>>()
		{
			{ typeof(User), () => $"{m_peopleStrings.Random()} {m_peopleStrings.Random()} {StringExtensions.RandomString(6)}" },
			{ typeof(Rebellion), () => m_rebellionStrings.Random() },
			{ typeof(WorkingGroup), () => m_wgStrings.Random() },
			{ typeof(LocalGroup), () => m_lgStrings.Random() },
			{ typeof(Role), () => m_roleStrings.Random() },
			{ typeof(Event), () => m_eventStrings.Random() },
			{ typeof(Site), () => m_siteStrings.Random() },
		};

		public static RoleSchema GetRandomRole(AccessContext context, GroupResource wg = null)
		{
			wg = wg ?? ResourceUtility.GetFactory<WorkingGroup>().CreateTypedObject(
				new ResourceCreationRequest(context, GetRandomWorkinGroup(context)));
			return new RoleSchema(RandomName<Role>(),
				SampleDescription, SampleRoleInstructions,
				wg.Guid);
		}

		public static EventSchema GetRandomEvent(AccessContext context, GroupResource rb = null)
		{
			rb = rb ?? ResourceUtility.GetFactory<Rebellion>().CreateTypedObject(
				new ResourceCreationRequest(context, GetRandomRebellion(context)));
			var date = RandomDate;
			return new EventSchema(RandomName<Event>(), rb.Guid, RandomLocation, SampleDescription, date, date + TimeSpan.FromHours(4));
		}

		public static SiteSchema GetRandomSite(AccessContext context, Rebellion rb = null)
		{
			rb = rb ?? ResourceUtility.GetFactory<Rebellion>().CreateTypedObject(
				new ResourceCreationRequest(context, GetRandomRebellion(context)));
			return new SiteSchema(RandomName<Site>(), rb.Guid, RandomLocation, SampleDescription);
		}

		public static UserSchema GetRandomUser(AccessContext context = default)
		{
			var nm = RandomName<User>();
			return new UserSchema(nm,
				ValidationExtensions.StripStringForSlug(nm),
				ValidationExtensions.GenerateStrongPassword(),
				$"{StringExtensions.RandomString(16)}@{StringExtensions.RandomString(16)}.com"
			);
		}

		public static RebellionSchema GetRandomRebellion(AccessContext context)
		{
			var rebellionNames = new[]
			{
				"London Uprising",
				"Paris Spring Rebellion",
				"Amsterdam Rebellion",
				"Sydney Week of Anger",
				"New York United",
				"L.A. Occupation",
				"Mexico City Resists"
			};
			var startDate = RandomDate;
			return new RebellionSchema(rebellionNames.Random(),
				SampleDescription,
				RandomLocation,
				startDate,
				startDate + TimeSpan.FromDays(m_random.NextDouble() * 14)
			);
		}

		public static WorkingGroupSchema GetRandomWorkinGroup(AccessContext context, GroupResource rsc = null)
		{
			var workingGroups = new[]
			{
				( "Action Design", "Gives advice on the design of how the whole action/actions are working, helps local groups with training and direction where needed. Keeps an over all eye on the functioning of actions, taking live feedback and helping groups to respond. Ensures Arrestee Support included in any action planning" ),
				( "Disability", "Ensures accessibility is embedded into the design and implementation of Actions, and focusing on the wellbeing of our disabled Rebels and the public."),
				( "First Aid", "To find enough first aiders for the Rebellion. This is a specialised skill that we cannot provide, and must look outside of the movement to provide." ),
				( "Legal Observers", "To organise trainings for those wanting to be Legal Observers, to coordinate them on the ground, and to ensure a non-partial relationship with LOs."),
				( "Police Liaison", "Gaining necessary information about an action, communicating it with the police and vice versa. Main priority is to protect organisers, by being the point of communication police should direct any questions or discussion topics to, rather than organisers themselves." ),
				( "Site Design & Build", "Draw together the skilled crew, the equipment, the content & procedures/briefing for sites and programmes which Tell the Truth and support rebels to rebel non-violently in safety"),
				("Arts", "Arrange, acquire structures needed for sites - incl stages - and ensure transport, safe rigging, use and de-rigging Design layout of sites & location of structures in consideration of production needs and action strategy Design look/feel of sites in consideration of Art/Messaging strategy"),
				("Sustenance", "Create sort maintain and manage recycling/waste streams (system and team) at rebellion sites. Create/locate and maintain toilets, manage the appropriate disposal of effluent."),
				("Transport & Storage", "Sourcing and installing kitchens - Sourcing food - Finding and scheduling cooking teams - Coordinate with RSO sustenance team - plan food provision in advance"),
				("Families", "Fixed overnight camping oversight - coordinating in with safety, cleanliess, stewarding etc to ensure a safe and healthy environment. Maintains contact with RSO Accomodation team to give information to rebels on the ground about what indoor accomodation is available around the city."),
				("Sanctuaries", "Provide a safe and restorative space for holistic, healing group workshops and therapies."),
				("Spokespeople & Training", "Ensures accessibility is embedded into the design and implementation of Production and create safe area for disabled camping and a disabled hub. Ensure coordination & communication with all coordination areas and with disabled people in the run up to and during the rebellion"),
				("Press", "To liaise with media on the ground and in back office. Working with actions teams to collect information for press releases, make sure media alerted to specific actions. Communicating with media points on the ground each morning and eve to receive updates. Arranging media appearances for spokes/notables. Current mandate in SOS system. "),
				("Leafletting & Outreach", "Creates and maintains an ongoing rota of all roles throughout the rebellion. Processes rota updates from sites and updates spreadsheet accordingly. "),
				("Organisational Systems Tech", " Reactive LINK person needed at each site to be available on a WhatsApp chat for: updates of sites and as an emergency point of contact.Also to hold a signposting role for others in the site with a limited list of contacts they can call on. Not a large role and could be shared. Non/low arrestable person preferred"),
				("Project Weaver Team", "Creates and manages the inter-site organisational system, creates educational materials on the system, provides advice on its use by regions and MoM groups, create lines of communication between groups where this is failing ")

			};
			rsc = rsc ?? ResourceUtility.GetFactory<Rebellion>().CreateTypedObject(
				new ResourceCreationRequest(context, GetRandomRebellion(context)));
			var rand = workingGroups.Random();
			return new WorkingGroupSchema(rand.Item1, rand.Item2, rsc.Guid);
		}

		public static LocalGroupSchema GetRandomLocalGroup(AccessContext context, GroupResource rsc = null)
		{
			var localGroups = new[]
			{
				"Paris", "Rome", "London", "New York", "Sydney", "Amsterdam", "Dublin", "Chicago", "Mexico City", "Santiago", "Istanbul", "Moscow"
			};
			rsc = rsc ?? ResourceUtility.GetFactory<Rebellion>().CreateTypedObject(
				new ResourceCreationRequest(context, GetRandomRebellion(context)));
			return new LocalGroupSchema(localGroups.Random(),
				SampleDescription,
				RandomLocation
			);
		}
	}
}
