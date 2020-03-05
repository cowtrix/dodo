using Common;
using Common.Extensions;
using Dodo;
using Dodo.Rebellions;
using Dodo.SharedTest;
using Dodo.Users;
using DodoTest.Framework.Postman;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources;
using Resources.Security;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace SharedTest
{
	[TestClass]
	public abstract class TestBase
	{
		protected static TestContext Context;
		private Random m_random = new Random();
		protected static PostmanCollection m_postman = new PostmanCollection("8888079-57fb4f3e-b2ad-4afe-a429-47a38866c5cd");

		protected IResourceManager<User> UserManager => ResourceUtility.GetManager<User>();

		[AssemblyInitialize]
		public static void SetupTests(TestContext testContext)
		{
			ResourceUtility.ClearAllManagers();
			Context = testContext;
			Logger.OnLog += OnLog;
		}

		private static void OnLog(string message, ELogLevel logLevel)
		{
			Context.WriteLine(message);
		}
	
		/// <summary>
		///  Gets or sets the test context which provides
		///  information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get { return Context; }
			set { Context = value; }
		}

		[TestCleanup]
		public void Clean()
		{
			ResourceUtility.ClearAllManagers();
		}

		[AssemblyCleanup]
		public static void Finalise()
		{
			m_postman.Update();
		}

		protected User GetRandomUser(out string password, out AccessContext context)
		{
			var schema = (UserSchema)SchemaGenerator.GetRandomSchema<User>(default);
			password = schema.Password;
			return GenerateUser(schema, out context);
		}

		protected virtual T CreateObject<T>(AccessContext context = default, ResourceSchemaBase schema = null) where T: IRESTResource
		{
			if (context.User == null)
			{
				GetRandomUser(out var password, out context); ;
			}
			if(schema == null)
			{
				schema = SchemaGenerator.GetRandomSchema<T>(context);
			}
			var factory = ResourceUtility.GetFactory<T>();
			return factory.CreateTypedObject(context, schema);
		}

		long LongRandom(long min, long max)
		{
			long result = m_random.Next((Int32)(min >> 32), (Int32)(max >> 32));
			result = (result << 32);
			result = result | (long)m_random.Next((Int32)min, (Int32)max);
			return result;
		}

		protected static User GenerateUser(UserSchema schema, out AccessContext context)
		{
			var userFactory = ResourceUtility.GetFactory<User>();
			var user = userFactory.CreateTypedObject(default(AccessContext), schema);
			context = new AccessContext(user, new Passphrase(user.AuthData.PassPhrase.GetValue(new Passphrase(schema?.Password))));
			Assert.IsTrue(context.Challenge());
			return user;
		}
	}
}
