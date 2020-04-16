using Common;
using Common.Config;
using SendGrid;
using SendGrid.Helpers.Mail;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dodo.Utility
{
	public static class EmailHelper
	{
#if DEBUG
		public static List<SendGridMessage> EmailHistory = new List<SendGridMessage>();
#endif

		static ConfigVariable<string> m_emailFrom = new ConfigVariable<string>("Email_FromEmail", $"noreply@{Dodo.PRODUCT_NAME}.com");
		static ConfigVariable<string> m_nameFrom = new ConfigVariable<string>("Email_FromName", $"{Dodo.PRODUCT_NAME} SysAdmin");
		static ConfigVariable<string> m_privacyPolicy = new ConfigVariable<string>("PrivacyPolicyURL", "http://www.todo.com/privacypolicy");
		static ConfigVariable<string> m_sendGridAPIKey = new ConfigVariable<string>("SendGrid_APIKey", "");
		static SendGridClient m_client;

		static Dictionary<string, string> StandardTemplate => new Dictionary<string, string>()
		{
			{ "product_name", Dodo.PRODUCT_NAME },
			{ "privacy_policy", m_privacyPolicy.Value }
		};

		static EmailHelper()
		{
			Logger.Debug($"Using SendGrid API Key {m_sendGridAPIKey.Value}");
			m_client = new SendGridClient(m_sendGridAPIKey.Value);
		}

		public static void SendEmail(string targetEmail, string targetName, string subject, string content)
		{
			var from = new EmailAddress(m_emailFrom.Value, m_nameFrom.Value);
			var to = new EmailAddress(targetEmail, targetName);
			SendEmail(MailHelper.CreateSingleEmail(from, to, subject, content, content));
		}

		public static void SendEmailVerificationEmail(string targetEmail, string targetName, string callback)
		{
			var from = new EmailAddress(m_emailFrom.Value, m_nameFrom.Value);
			var to = new EmailAddress(targetEmail, targetName);
			var dynamicTemplateData = StandardTemplate;
			StandardTemplate["name"] = targetName;
			StandardTemplate["callback"] = callback;
			SendEmail(MailHelper.CreateSingleTemplateEmail(from, to, "d-abb66e4f174c470abeb5e6a1ecdaac85", dynamicTemplateData));
		}

		private static void SendEmail(SendGridMessage msg)
		{
#if DEBUG
			Logger.Warning("Sending of email suppressed due to debug mode");
			EmailHistory.Add(msg);
			return;
#endif
			var t = new Task(async () =>
			{
				var response = await m_client.SendEmailAsync(msg);
				if(response.StatusCode != HttpStatusCode.OK)
				{
					Logger.Error($"Failed to send email message, error code {response.StatusCode}: {await response.Body.ReadAsStringAsync()}");
				}
				Logger.Debug($"Sent email to SendGrid API: {response.StatusCode}: {await response.Body.ReadAsStringAsync()}");
			});
			t.Start();
		}
	}
}
