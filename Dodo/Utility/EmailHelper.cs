using Common;
using Common.Config;
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
		static ConfigVariable<string> m_emailFrom = new ConfigVariable<string>("Email_FromEmail", "noreply@dodo.earth");
		static ConfigVariable<string> m_nameFrom = new ConfigVariable<string>("Email_FromName", "Dodo SysAdmin");
		static ConfigVariable<string> m_sendGridAPIKey = new ConfigVariable<string>("SendGrid_APIKey", "");
		static SendGridClient m_client;

		static EmailHelper()
		{
			m_client = new SendGridClient(m_sendGridAPIKey.Value);
		}

		public static void SendEmail(string targetEmail, string targetName, string subject, string content)
		{
			var from = new EmailAddress(m_emailFrom.Value, m_nameFrom.Value);
			var to = new EmailAddress(targetEmail, targetName);
			SendAsync(MailHelper.CreateSingleEmail(from, to, subject, content, content));
		}

		private static void SendAsync(SendGridMessage msg)
		{
			var t = new Task(async () => await m_client.SendEmailAsync(msg));
			t.Start();
		}
	}
}
