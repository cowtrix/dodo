using Dodo.Users;
using OpenQA.Selenium;

namespace Dodo.SeleniumTests
{
	public class RegisterPageHandler : SeleniumPageHandler
	{
		public IWebElement Username => Driver.WaitForElement(By.Id("username"));
		public IWebElement Email => Driver.WaitForElement(By.Id("email"));
		public IWebElement Password => Driver.WaitForElement(By.Id("password"));
		public IWebElement PasswordConfirm => Driver.WaitForElement(By.Id("confirmPassword"));
		public IWebElement Checkbox_NewNotifications => Driver.WaitForElement(By.Id("newNotifications"));
		public IWebElement Checkbox_PrivacyPolicy => Driver.WaitForElement(By.Id("termsConditions"));
		public IWebElement Submit => Driver.WaitForElement(By.CssSelector("button[class*='button_button']"));

		public RegisterPageHandler(IWebDriver driver) : base(driver)
		{
		}

		public void DoRegister(UserSchema schema)
		{
			Username.SendKeys(schema.Username);
			Email.SendKeys(schema.Email);
			Password.SendKeys(schema.Password);
			PasswordConfirm.SendKeys(schema.Password);
			if(schema.NewNotifications)
			{
				Checkbox_NewNotifications.Click();
			}
			Checkbox_PrivacyPolicy.Click();
			Submit.Click();
		}
	} 
}
