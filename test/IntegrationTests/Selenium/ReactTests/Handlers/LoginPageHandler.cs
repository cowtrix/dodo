using OpenQA.Selenium;

namespace Dodo.SeleniumTests
{
	public class LoginPageHandler : SeleniumPageHandler
	{
		public IWebElement Username => Driver.WaitForElement(By.Id("username"));
		public IWebElement Password => Driver.WaitForElement(By.Id("password"));
		public IWebElement Submit => Driver.WaitForElement(By.CssSelector("button[class*='button_button']"));

		public LoginPageHandler(IWebDriver driver) : base(driver)
		{
		}

		public void DoLogin(string slug, string pass, bool rememberMe)
		{
			Username.SendKeys(slug);
			Password.SendKeys(pass);
			Submit.Click();
		}
	}
}
