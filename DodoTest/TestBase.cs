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
	}
}
