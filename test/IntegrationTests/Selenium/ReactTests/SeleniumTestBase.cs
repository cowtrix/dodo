using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using OpenQA.Selenium;
using OpenQA.Selenium.Firefox;
using SharedTest;
using Microsoft.AspNetCore.TestHost;
using Microsoft.AspNetCore.Hosting;
using Microsoft.Extensions.Hosting;
using System.Linq;

namespace Dodo.SeleniumTests
{
	public abstract class SeleniumTestBase : TestBase
	{
		protected const string URL = "https://localhost:5001";
		protected IWebDriver Driver;

		protected Cookie LoginCookie => Driver.Manage().Cookies.AllCookies.SingleOrDefault(c => c.Name == ".AspNetCore.Cookies");

		protected void Login(Models.LoginModel model)
		{
			Driver.Navigate().GoToUrl($"{URL}/login");
			var loginPage = new LoginPageHandler(Driver);
			loginPage.DoLogin(model.Username, model.Password, true);
		}

		protected void SkipIfNot<TBase, TParent>()
		{
			if (!typeof(TParent).IsAssignableFrom(typeof(TBase)))
			{
				Assert.Inconclusive();
			}
		}

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
			foreach (var p in System.Diagnostics.Process.GetProcessesByName("geckodriver"))
				p.Kill();

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
