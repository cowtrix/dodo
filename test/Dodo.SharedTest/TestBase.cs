using Common;
using Common.Config;
using Dodo;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.SharedTest;
using Dodo.LocationResources;
using Dodo.Users;
using Dodo.Users.Tokens;
using Dodo.WorkingGroups;
using DodoTest.Framework.Postman;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mongo2Go;
using Resources;
using Resources.Security;
using System;
using Dodo.LocalGroups;

namespace SharedTest
{
	[TestClass]
	public abstract class TestBase
	{
		private static MongoDbRunner m_runner;
		private static TestContext m_context;
		private Random m_random = new Random();
		protected static PostmanCollection Postman { get; private set; } = new PostmanCollection(ConfigManager.GetValue($"{nameof(TestBase)}_PostmanCollection", ""));
		protected static IResourceManager<User> UserManager => ResourceUtility.GetManager<User>();

		[AssemblyInitialize]
		public static void SetupTests(TestContext testContext)
		{
			m_context = testContext;
		}

		public TestBase()
		{
			if (m_runner != null)
			{
				return;
			}
			m_runner = MongoDbRunner.Start();
			ConfigManager.SetValue(ResourceUtility.CONFIGKEY_MONGODBSERVERURL, m_runner.ConnectionString);
			ResourceUtility.ClearAllManagers();
			Dodo.Security.SessionTokenStore.Initialise();
			Logger.OnLog += OnLog;
		}

		private static void OnLog(LogMessage message)
		{
			m_context?.WriteLine(message.ToString());
		}

		/// <summary>
		///  Gets or sets the test context which provides
		///  information about and functionality for the current test run.
		///</summary>
		public static TestContext TestContext
		{
			get { return m_context; }
			set { m_context = value; }
		}

		[TestCleanup]
		public void Clean()
		{
			ResourceUtility.ClearAllManagers();
		}

		[AssemblyCleanup]
		public static void Finalise()
		{
			Postman.Update();
			m_runner?.Dispose();
			m_runner = null;
		}

		public static User GetRandomUser(out string password, out AccessContext context, bool verifyEmail = true)
		{
			var schema = (UserSchema)SchemaGenerator.GetRandomSchema<User>(default);
			password = schema.Password;
			return GenerateUser(schema, out context, verifyEmail);
		}

		protected virtual T CreateObject<T>(AccessContext context = default, ResourceSchemaBase schema = null, bool seed = true, bool publish = true) where T : IRESTResource
		{
			return CreateNewObject<T>(context, schema, seed, publish);
		}

		public static T CreateNewObject<T>(AccessContext context = default, ResourceSchemaBase schema = null, bool seed = true, bool publish = true) where T : IRESTResource
		{
			if (context.User == null)
			{
				GetRandomUser(out var password, out context, true); ;
			}
			if (schema == null)
			{
				schema = SchemaGenerator.GetRandomSchema<T>(context);
			}
			var factory = ResourceUtility.GetFactory<T>();
			var obj = factory.CreateTypedObject(new ResourceCreationRequest(context, schema));
			if (publish && obj is IPublicResource pubRsc)
			{
				obj = (T)pubRsc.Publish();
			}
			if (publish && obj is IAdministratedResource admin)
			{
				using (var rscLock = new ResourceLock(admin))
				{
					admin = rscLock.Value as IAdministratedResource;
					admin.AddNewAdmin(context, GetRandomUser(out _, out _));
					admin.AddNewAdmin(context, GetRandomUser(out _, out _));
					admin.AddNewAdmin(context, GetRandomUser(out _, out _));
					ResourceUtility.GetManager(admin.GetType()).Update(admin, rscLock);
				}
			}
			if (publish && obj is INotificationResource not)
			{
				using (var rscLock = new ResourceLock(not))
				{
					not = rscLock.Value as INotificationResource;
					not.TokenCollection.AddOrUpdate(not, new SimpleNotificationToken(context.User, null,
						"This is a test short Public announcement.", null, ENotificationType.Announcement, EPermissionLevel.PUBLIC, true));
					not.TokenCollection.AddOrUpdate(not, new SimpleNotificationToken(context.User, null,
						"This is a test short Admin only announcement.", null, ENotificationType.Announcement, EPermissionLevel.ADMIN, true));
					not.TokenCollection.AddOrUpdate(not, new SimpleNotificationToken(context.User, null,
						"This is a test short Members only announcement.", null, ENotificationType.Announcement, EPermissionLevel.MEMBER, true));
					not.TokenCollection.AddOrUpdate(not, new SimpleNotificationToken(context.User, null,
						"This is a longer Public announcement: " + SchemaGenerator.SampleDescription, null, ENotificationType.Announcement, EPermissionLevel.PUBLIC, true));
					ResourceUtility.GetManager(not.GetType()).Update(not, rscLock);
				}
			}
			if (seed)
			{
				if (obj is Rebellion rebellion)
				{
					rebellion.VideoEmbedURL = SchemaGenerator.RandomVideoURL;

					// Add some working groups, sites
					CreateNewObject<WorkingGroup>(context, SchemaGenerator.GetRandomWorkinGroup(context, rebellion));
					CreateNewObject<WorkingGroup>(context, SchemaGenerator.GetRandomWorkinGroup(context, rebellion));

					CreateNewObject<Site>(context, SchemaGenerator.GetRandomSite(context, rebellion));
					CreateNewObject<Site>(context, SchemaGenerator.GetRandomSite(context, rebellion));

					CreateNewObject<Event>(context, SchemaGenerator.GetRandomEvent(context, rebellion));
					CreateNewObject<Event>(context, SchemaGenerator.GetRandomEvent(context, rebellion));
				}
				else if (obj is WorkingGroup wg)
				{
					if (wg.Parent.Type == nameof(Rebellion))
					{
						CreateNewObject<WorkingGroup>(context, SchemaGenerator.GetRandomWorkinGroup(context, wg));
						CreateNewObject<WorkingGroup>(context, SchemaGenerator.GetRandomWorkinGroup(context, wg));
					}
					CreateNewObject<Role>(context, SchemaGenerator.GetRandomRole(context, wg));
					CreateNewObject<Role>(context, SchemaGenerator.GetRandomRole(context, wg));
				}
				else if (obj is LocalGroup lg)
				{
					CreateNewObject<WorkingGroup>(context, SchemaGenerator.GetRandomWorkinGroup(context, lg));
					CreateNewObject<WorkingGroup>(context, SchemaGenerator.GetRandomWorkinGroup(context, lg));

					CreateNewObject<Event>(context, SchemaGenerator.GetRandomEvent(context, lg));
					CreateNewObject<Event>(context, SchemaGenerator.GetRandomEvent(context, lg));
				}
				else if (obj is Role role)
				{
					int appCount = new Random().Next(0, 3);
					using (var rscLock = new ResourceLock(role))
					{
						for (var i = 0; i < appCount; ++i)
						{
							var applicant = GetRandomUser(out _, out var appcontext);
							role.Apply(appcontext, new ApplicationModel() { Content = "Hello, this is a test application!" }, out _);

						}
						ResourceUtility.GetManager(role.GetType()).Update(role, rscLock);
					}
				}
			}
			return ResourceUtility.GetManager<T>().GetSingle(r => r.Guid == obj.Guid);
		}

		long LongRandom(long min, long max)
		{
			long result = m_random.Next((Int32)(min >> 32), (Int32)(max >> 32));
			result = (result << 32);
			result = result | (long)m_random.Next((Int32)min, (Int32)max);
			return result;
		}

		public static User GenerateUser(UserSchema schema, out AccessContext context, bool verifyEmail = true)
		{
			if (schema == null)
			{
				throw new ArgumentNullException(nameof(schema));
			}
			var userFactory = ResourceUtility.GetFactory<User>();
			var user = userFactory.CreateTypedObject(new ResourceCreationRequest(default, schema));
			var passphrase = new Passphrase(user.AuthData.PassPhrase.GetValue(new Passphrase(schema.Password)));
			context = new AccessContext(user, passphrase);
			Assert.IsTrue(context.Challenge());

			// Verify email if flag has been set
			if (verifyEmail)
			{
				using(var rscLock = new ResourceLock(user.Guid))
				{
					var verifyToken = user.TokenCollection.GetSingleToken<VerifyEmailToken>(context, EPermissionLevel.OWNER, user);
					Assert.IsNotNull(verifyToken);
					verifyToken.Redeem(context);
					user.PersonalData.EmailConfirmed = true;
					UserManager.Update(user, rscLock);
				}
			}
			return user;
		}

	}
}
