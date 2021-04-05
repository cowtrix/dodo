using OpenQA.Selenium;

namespace Dodo.SeleniumTests
{
	public abstract class SeleniumPageHandler
	{
		public IWebDriver Driver { get; private set; }

		public SeleniumPageHandler(IWebDriver driver) { Driver = driver; }
	}
}
