using OpenQA.Selenium;

namespace Dodo.SeleniumTests
{
	public class ResourcePageHandler : SeleniumPageHandler
	{
		public IWebElement Title => Driver.WaitForElement(By.CssSelector("div[class^='header_'] > div > h1"));
		public IWebElement Date => Driver.WaitForElement(By.CssSelector("div[class^='header_'] > h3"));
		public IWebElement Address => Driver.WaitForElement(By.CssSelector("div[class^='header_'] > div > h4"));
		public IWebElement Description => Driver.WaitForElement(By.CssSelector("div[class^='description_description']"));
		public IWebElement CountdownContainer => Driver.WaitForElement(By.CssSelector("div[class^='countdown_countdown']"));
		public IWebElement SubscribeButton => Driver.WaitForElement(By.CssSelector("div[class^='sign-up-button_wrapper']"));

		public ResourcePageHandler(IWebDriver driver) : base(driver)
		{
		}
	}
}
