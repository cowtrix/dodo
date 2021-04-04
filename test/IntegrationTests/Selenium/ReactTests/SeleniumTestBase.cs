using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SharedTest;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;

namespace Dodo.SeleniumTests
{
	public abstract class SeleniumTestBase : TestBase
	{
		protected const string URL = "https://localhost:5001";
		protected IWebDriver Driver;

		public SeleniumTestBase()
		{
			new Task( () => DodoServer.DodoServer.CreateHostBuilder($"--urls {URL}")			
				.Build()
				.Run())
				.Start();
			// Todo: a better solution to setup
			System.Threading.Thread.Sleep(5000);
		}

		[TestInitialize]
		public void OpenBrowser()
		{
			Driver = new FirefoxDriver(
				new FirefoxOptions
				{
					AcceptInsecureCertificates = true,
					
				});
		}

		[TestCleanup]
		public void CleanupBrowser()
		{
			Driver?.Close();
		}
	}
}
