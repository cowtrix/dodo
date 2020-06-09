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

		static ConfigVariable<string> m_emailFrom = new ConfigVariable<string>("Email_FromEmail", $"noreply@{DodoApp.PRODUCT_NAME}.com");
		static ConfigVariable<string> m_nameFrom = new ConfigVariable<string>("Email_FromName", $"{DodoApp.PRODUCT_NAME} SysAdmin");
		static ConfigVariable<string> m_privacyPolicy = new ConfigVariable<string>("PrivacyPolicyURL", "http://www.todo.com/privacypolicy");
		static ConfigVariable<string> m_sendGridAPIKey = new ConfigVariable<string>("SendGrid_APIKey", "");

		static ConfigVariable<string> m_verifyEmailTemplateGUID = new ConfigVariable<string>($"{nameof(EmailHelper)}_SendGridTemplate_VerifyEmail", "d-abb66e4f174c470abeb5e6a1ecdaac85");
		static ConfigVariable<string> m_resetPasswordTemplateGUID = new ConfigVariable<string>($"{nameof(EmailHelper)}_SendGridTemplate_ResetPassword", "d-43861bc9e0dd44b29b131da9b10a07d1");

		static SendGridClient m_client;

		static Dictionary<string, string> GetStandardTemplate() => new Dictionary<string, string>()
		{
			{ "product_name", DodoApp.PRODUCT_NAME },
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

		public static void SendPasswordResetEmail(string targetEmail, string targetName, string callback)
		{
			SendCallbackEmail(targetEmail, targetName, callback, m_resetPasswordTemplateGUID.Value);
		}

		public static void SendEmailVerificationEmail(string targetEmail, string targetName, string callback)
		{
			SendCallbackEmail(targetEmail, targetName, callback, m_verifyEmailTemplateGUID.Value);
		}

		private static void SendCallbackEmail(string targetEmail, string targetName, string callback, string template)
		{
			var from = new EmailAddress(m_emailFrom.Value, m_nameFrom.Value);
			var to = new EmailAddress(targetEmail, targetName);
			var dynamicTemplateData = GetStandardTemplate();
			dynamicTemplateData["name"] = targetName;
			dynamicTemplateData["callback"] = callback;
			SendEmail(MailHelper.CreateSingleTemplateEmail(from, to, template, dynamicTemplateData));
		}

		private static void SendEmail(SendGridMessage msg)
		{
#if DEBUG
			EmailHistory.Add(msg);
#endif
			if(string.IsNullOrEmpty(m_sendGridAPIKey.Value))
			{
				Logger.Error($"No SendGrid API key was set - suppressing email send");
				return;
			}
			var t = new Task(async () =>
			{
				var response = await m_client.SendEmailAsync(msg);
				if(response.StatusCode != HttpStatusCode.Accepted)
				{
					Logger.Error($"Failed to send email message, error code {response.StatusCode}: Body:[{await response.Body.ReadAsStringAsync()}]");
				}
			});
			t.Start();
		}
	}
}
