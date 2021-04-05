using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using Dodo.Rebellions;
using System;
using System.Linq;
using OpenQA.Selenium;
using Dodo.SharedTest;

namespace Dodo.SeleniumTests
{

	[TestClass]
	public class HomepageTests : SeleniumTestBase
	{
		[TestMethod]
		public void CanLogin()
		{
			Driver.Url = URL;

			var navbar = new AnonHomepageHandler(Driver);
			Assert.IsNotNull(navbar.Login);

			var user = GetRandomUser(out var pass, out var context);
			navbar.Login.Click();

			// At login page
			Assert.AreEqual($"{Dodo.DodoApp.NetConfig.FullURI}/login", Driver.Url);
			var loginPage = new LoginPageHandler(Driver);

			loginPage.DoLogin(user.Slug, pass, true);

			var loggedinNavbar = new LoggedHomepageHandler(Driver);
			
			Assert.IsNotNull(loggedinNavbar.UserMenuButton);
			Assert.IsNotNull(LoginCookie);
		}

		[TestMethod]
		public void CanRegister()
		{
			Driver.Url = $"{URL}/register";

			var schema = SchemaGenerator.GetRandomUser();
			var register = new RegisterPageHandler(Driver);
			register.DoRegister(schema);

			var loggedinNavbar = new LoggedHomepageHandler(Driver);

			Assert.IsNotNull(loggedinNavbar.UserMenuButton);
			Assert.IsNotNull(LoginCookie);
		}

		[TestMethod]
		public void CanAcceptCookies()
		{
			Driver.Url = URL;

			var cookieButton = Driver.TryFindElement(By.Id("rcc-confirm-button"));
			Assert.IsNotNull(cookieButton);
			cookieButton.Click();

			var cookie = Driver.Manage().Cookies.AllCookies.SingleOrDefault(c => c.Name == "CookieConsent");
			Assert.IsNotNull(cookie);
			Assert.AreEqual("true", cookie.Value);
		}

		[TestMethod]
		public void CanUseMapMarkers()
		{
			var rebellion = CreateNewObject<Rebellion>(seed: false);

			Driver.Url = URL;

			var homepage = new AnonHomepageHandler(Driver);
			Assert.IsNotNull(homepage.MapContainer);

			// Find single marker of rebellion, should be center screen
			var marker = homepage.Markers.SingleOrDefault();
			Assert.IsNotNull(marker);
			marker.Click();

			// Ensure that clicking it brought up a tooltip
			var tooltip = Driver.WaitForElement(By.ClassName("leaflet-popup-content"));
			Assert.IsNotNull(tooltip);
			tooltip.Click();

			// Ensure that clicking the tooltip took us to the rebellion page
			var rsc = new ResourcePageHandler(Driver);
			Assert.AreEqual(rebellion.Name, rsc.Title.Text);
		}
	}
}
