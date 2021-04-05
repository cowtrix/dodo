using OpenQA.Selenium;
using System.Collections.Generic;

namespace Dodo.SeleniumTests
{
	public abstract class HomepageHandler : SeleniumPageHandler
	{
		public IWebElement MapContainer => Driver.WaitForElement(By.ClassName("leaflet-container"));
		public IEnumerable<IWebElement> Markers => Driver.FindElements(By.ClassName("xrMarker"));

		public HomepageHandler(IWebDriver driver) : base(driver)
		{
		}
	}

	public class AnonHomepageHandler : HomepageHandler
	{
		public IWebElement Login => Driver
			.WaitForElement(By.CssSelector("a[class^='login-register_loginRegister__']"));

		public AnonHomepageHandler(IWebDriver driver) : base(driver)
		{
		}
	}

	public class LoggedHomepageHandler : SeleniumPageHandler
	{
		public IWebElement UserMenuButton => Driver
			.WaitForElement(By.CssSelector("img[class^='user-button_userButton']"));

		public LoggedHomepageHandler(IWebDriver driver) : base(driver)
		{
		}
	}
}
