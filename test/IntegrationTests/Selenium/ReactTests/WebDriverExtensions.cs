using OpenQA.Selenium;
using OpenQA.Selenium.Interactions;
using System;
using System.Diagnostics;

namespace Dodo.SeleniumTests
{
	public static class WebDriverExtensions
	{
		public static IWebElement WaitForElement(this IWebDriver driver, By by, TimeSpan? timeout = null)
		{
			var sw = Stopwatch.StartNew();
			if (timeout == null)
				timeout = TimeSpan.FromSeconds(2);
			IWebElement result = null;
			while (sw.Elapsed < timeout && result == null)
			{
				result = driver.TryFindElement(by);
			}
			return result;
		}

		public static IWebElement TryFindElement(this IWebDriver driver, By by)
		{
			try
			{
				return driver.FindElement(by);
			}
			catch (NoSuchElementException e) { }
			return null;
		}

		public static IJavaScriptExecutor JS(this IWebDriver driver) => driver as IJavaScriptExecutor;

		public static void ScrollToElement(this IWebDriver driver, IWebElement element)
		{
			driver.JS().ExecuteScript("arguments[0].scrollIntoView(false)", element);
		}

		public static void Scroll(this IWebDriver driver, int x, int y)
		{
			driver.JS().ExecuteScript($"window.scrollBy({x},{y})");
		}
	}
}
