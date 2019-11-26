using Common;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodo.Utility
{
	public static class EmailHelper
	{
		static ConfigVariable<string> m_sendGridAPIKey = new ConfigVariable<string>("SendGrid_APIKey", "");

		public static async Task Execute()
		{
			var client = new SendGridClient(m_sendGridAPIKey.Value);
			var from = new EmailAddress("test@example.com", "Example User");
			var subject = "Sending with Twilio SendGrid is Fun";
			var to = new EmailAddress("seandgfinnegan@gmail.com", "Example User");
			var plainTextContent = "and easy to do anywhere, even with C#";
			var htmlContent = "<strong>and easy to do anywhere, even with C#</strong>";
			var msg = MailHelper.CreateSingleEmail(from, to, subject, plainTextContent, htmlContent);
			var response = await client.SendEmailAsync(msg);
		}
	}
}
