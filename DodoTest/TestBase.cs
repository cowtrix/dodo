using Microsoft.VisualStudio.TestTools.UnitTesting;
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

		[TestInitialize]
		public void Clean()
		{
			DodoServer.CoordinatorNeedsManager.ClearAll();
		}

		public User GetTestUser(EUserAccessLevel accessLevel)
		{
			var ph = "441315103992";
			var telegramID = 997875;
			var user = new User()
			{
				Name = "Test",
				PhoneNumber = ph,
				TelegramUser = telegramID,
			};
			if (accessLevel == EUserAccessLevel.Coordinator)
			{
				var wg = DodoServer.SiteManager.GenerateWorkingGroup("Test", EParentGroup.MovementSupport, "Test working group");
				DodoServer.SiteManager.Data.WorkingGroups.Add(wg.ShortCode, wg);
				user.CoordinatorRoles.Add(new Role(wg, "Test role", 3));
			}
			else if (accessLevel == EUserAccessLevel.RotaCoordinator)
			{
				var wg = DodoServer.SiteManager.GenerateWorkingGroup("Test Rota", EParentGroup.MovementSupport, "Test rota working group");
				DodoServer.SiteManager.Data.WorkingGroups.Add(wg.ShortCode, wg);
				user.CoordinatorRoles.Add(new Role(wg, "Test role", 1));
			}
			else if (accessLevel == EUserAccessLevel.RSO)
			{
				var wg = DodoServer.SiteManager.GenerateWorkingGroup("Test RSO", EParentGroup.MovementSupport, "Test RSO working group");
				DodoServer.SiteManager.Data.WorkingGroups.Add(wg.ShortCode, wg);
				user.CoordinatorRoles.Add(new Role(wg, "Test role", 0));
			}
			return user;
		}
	}
}
