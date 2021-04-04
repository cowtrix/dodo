using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Threading.Tasks;
using OpenQA.Selenium;
using Dodo.Rebellions;
using System;

namespace Dodo.SeleniumTests
{
	[TestClass]
	public class BrowserHomeTests : SeleniumTestBase
	{
		[AssemblyInitialize]
		public static void SetupData(TestContext context)
		{
			try
			{
				CreateNewObject<Rebellion>();
			}
			catch(Exception e)
			{
				Console.WriteLine(e);
			}
		}

		[TestMethod]
		public async Task CanLoadIndex()
		{
			Driver.Url = URL;
			Assert.IsNotNull(Driver.FindElement(By.Id("root")));
		}
	}
}
