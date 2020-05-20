using Common;
using Common.Config;
using Dodo;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.SharedTest;
using Dodo.Sites;
using Dodo.Users;
using Dodo.Users.Tokens;
using Dodo.WorkingGroups;
using DodoTest.Framework.Postman;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Mongo2Go;
using Resources;
using Resources.Security;
using System;

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

		protected virtual T CreateObject<T>(AccessContext context = default, ResourceSchemaBase schema = null) where T : IRESTResource
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
			var obj = factory.CreateTypedObject(context, schema);
			if (obj is Rebellion rebellion)
			{
				// Add some working groups, sites
				CreateObject<WorkingGroup>(context, SchemaGenerator.GetRandomWorkinGroup(context, rebellion));
				CreateObject<WorkingGroup>(context, SchemaGenerator.GetRandomWorkinGroup(context, rebellion));

				CreateObject<Site>(context, SchemaGenerator.GetRandomSite(context, rebellion));
				CreateObject<Site>(context, SchemaGenerator.GetRandomSite(context, rebellion));
			}
			else if (obj is WorkingGroup wg && !(wg.Parent.GetValue() is Rebellion))
			{
				CreateObject<WorkingGroup>(context, SchemaGenerator.GetRandomWorkinGroup(context, wg));
				CreateObject<WorkingGroup>(context, SchemaGenerator.GetRandomWorkinGroup(context, wg));
				CreateObject<Role>(context, SchemaGenerator.GetRandomRole(context, wg));
				CreateObject<Role>(context, SchemaGenerator.GetRandomRole(context, wg));
			}
			return obj;
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
			var user = userFactory.CreateTypedObject(default(AccessContext), schema);
			var passphrase = new Passphrase(user.AuthData.PassPhrase.GetValue(new Passphrase(schema.Password)));
			context = new AccessContext(user, passphrase);
			Assert.IsTrue(context.Challenge());

			// Verify email if flag has been set
			if (verifyEmail)
			{
				var verifyToken = user.TokenCollection.GetSingleToken<VerifyEmailToken>(context);
				Assert.IsNotNull(verifyToken);
				user.PersonalData.EmailConfirmed = true;
			}

			return user;
		}

	}
}
