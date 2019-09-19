﻿using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using XR.Dodo;

namespace DodoTest
{
	[TestClass]
	public abstract class TestBase
	{
		private TestContext testContextInstance;
		protected SymSyncSimulator m_simulator;

		public TestBase()
		{
			DodoServer.Initialise("-d");
			m_simulator = new SymSyncSimulator("http://localhost:8080");
		}

		/// <summary>
		///  Gets or sets the test context which provides
		///  information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		[TestCleanup]
		public void Clean()
		{
			DodoServer.CoordinatorNeedsManager.ClearAll();
		}

		long LongRandom(long min, long max, Random rand)
		{
			long result = rand.Next((Int32)(min >> 32), (Int32)(max >> 32));
			result = (result << 32);
			result = result | (long)rand.Next((Int32)min, (Int32)max);
			return result;
		}

		public User GetTestUser(EUserAccessLevel accessLevel)
		{
			var r = new Random();
			var ph = "44" + LongRandom(1000000000L, 9999999999L, r).ToString(); // "1315103992";
			ValidationExtensions.ValidateNumber(ref ph);
			var telegramID = r.Next(10000, 99999);
			var user = new User()
			{
				Name = "Test" + Utility.RandomString(5, r.Next().ToString()),
				PhoneNumber = ph,
				TelegramUser = telegramID,
			};
			if (accessLevel == EUserAccessLevel.Coordinator)
			{
				var wg = DodoServer.SiteManager.GenerateWorkingGroup("Test", EParentGroup.MovementSupport, "Test working group");
				user.CoordinatorRoles.Add(new Role(wg, "Test role", 3));
			}
			else if (accessLevel == EUserAccessLevel.RotaCoordinator)
			{
				var wg = DodoServer.SiteManager.GenerateWorkingGroup("Test Rota", EParentGroup.MovementSupport, "Test rota working group");
				user.CoordinatorRoles.Add(new Role(wg, "Test role", 1));
			}
			else if (accessLevel == EUserAccessLevel.RSO)
			{
				var wg = DodoServer.SiteManager.GenerateWorkingGroup("Test RSO", EParentGroup.MovementSupport, "Test RSO working group");
				user.CoordinatorRoles.Add(new Role(wg, "Test role", 0));
			}
			var session = DodoServer.SessionManager.GetOrCreateSession(user);
			session.Workflow.CurrentTask.ExitTask();	// Just to skip intro
			return user;
		}
	}
}
