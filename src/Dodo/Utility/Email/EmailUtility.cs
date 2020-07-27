using Common;
using Common.Config;
using StrongGrid.Models;
using System;
using System.Collections.Generic;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dodo.Utility
{
	public static class EmailUtility
	{
#if DEBUG
		//public static List<Email> EmailHistory = new List<SendGridMessage>();
#endif

		static ConfigVariable<string> m_emailFrom = new ConfigVariable<string>("Email_FromEmail", $"noreply@{DodoApp.PRODUCT_NAME}.com");
		static ConfigVariable<string> m_nameFrom = new ConfigVariable<string>("Email_FromName", $"{DodoApp.PRODUCT_NAME} SysAdmin");
		static ConfigVariable<string> m_sendGridAPIKey = new ConfigVariable<string>("SendGrid_APIKey", "");

		static ConfigVariable<string> m_verifyEmailTemplateGUID = new ConfigVariable<string>($"{nameof(EmailUtility)}_SendGridTemplate_VerifyEmail", "d-abb66e4f174c470abeb5e6a1ecdaac85");
		static ConfigVariable<string> m_resetPasswordTemplateGUID = new ConfigVariable<string>($"{nameof(EmailUtility)}_SendGridTemplate_ResetPassword", "d-43861bc9e0dd44b29b131da9b10a07d1");
		static ConfigVariable<string> m_newApplicantTemplateGUID = new ConfigVariable<string>($"{nameof(EmailUtility)}_SendGridTemplate_NewApplicant", "d-582357f020794af095c55d31b4b924a2");

		static StrongGrid.Client m_client;

		static Dictionary<string, string> GetStandardTemplate() => new Dictionary<string, string>()
		{
			{ "product_name", DodoApp.PRODUCT_NAME },
			{ "privacy_policy", DodoApp.PrivacyPolicyURL },
			{ "url", DodoApp.NetConfig.FullURI },
		};

		static EmailUtility()
		{
			if (string.IsNullOrEmpty(m_sendGridAPIKey.Value))
			{
				Logger.Warning($"No {m_sendGridAPIKey.ConfigKey} specified, email will not work.");
				return;
			}
			Logger.Debug($"Using SendGrid API Key {m_sendGridAPIKey.Value}");
			m_client = new StrongGrid.Client(m_sendGridAPIKey.Value);
		}

		public static void SendEmail(string targetEmail, string targetName,
			string subject, string textContent, string htmlContent = null)
		{
			SendEmail(targetEmail, targetName, m_emailFrom.Value, m_nameFrom.Value, subject, textContent, htmlContent);
		}

		public static void SendEmail(string targetEmail, string targetName,
			string fromEmail, string fromName, string subject, string textContent, string htmlContent = null)
		{
			Logger.Debug($"Sending an email from {fromEmail} to {targetEmail}: {subject}");
			htmlContent = htmlContent ?? textContent;
			if (m_client == null)
			{
				Logger.Warning($"No {m_sendGridAPIKey.ConfigKey} specified, email send was suppressed.");
				return;
			}
			var from = new MailAddress(fromEmail, fromName);
			var to = new MailAddress(targetEmail, targetName);
			if (string.IsNullOrEmpty(m_sendGridAPIKey.Value))
			{
				Logger.Error($"No SendGrid API key was set - suppressing email send");
				return;
			}
			var t = new Task(async () =>
			{
				try
				{
					await m_client.Mail.SendToSingleRecipientAsync(to, from, subject, htmlContent, textContent,
						trackOpens: false, trackClicks: false); // Important!
				}
				catch (Exception e)
				{
					Logger.Exception(e, $"Failed to send email message");
				}
			});
			t.Start();
		}

		public static void SendPasswordResetEmail(string targetEmail, string targetName, string callback)
		{
			SendCallbackEmail(targetEmail, targetName, callback, m_resetPasswordTemplateGUID.Value);
		}

		public static void SendEmailVerificationEmail(string targetEmail, string targetName, string callback)
		{
			SendCallbackEmail(targetEmail, targetName, callback, m_verifyEmailTemplateGUID.Value);
		}

		public static void SendNewRoleApplicantEmail(string targetEmail, string fromEmail, 
			string roleUrl, string roleName, string question, string application)
		{
			var from = new MailAddress(fromEmail, Dodo.DodoApp.PRODUCT_NAME);
			var to = new MailAddress(targetEmail, Dodo.DodoApp.PRODUCT_NAME);
			var dynamicTemplateData = GetStandardTemplate();
			dynamicTemplateData["role_name"] = roleName;
			dynamicTemplateData["role_url"] = roleName;
			dynamicTemplateData["role_question"] = question;
			dynamicTemplateData["role_application"] = application;
			SendTemplateEmail(from, to, m_newApplicantTemplateGUID.Value, dynamicTemplateData);
		}

		private static void SendCallbackEmail(string targetEmail, string targetName, string callback, string template)
		{
			var from = new MailAddress(m_emailFrom.Value, m_nameFrom.Value);
			var to = new MailAddress(targetEmail, targetName);
			var dynamicTemplateData = GetStandardTemplate();
			dynamicTemplateData["name"] = targetName;
			dynamicTemplateData["callback"] = callback;
			SendTemplateEmail(from, to, template, dynamicTemplateData);
		}

		private static void SendTemplateEmail(MailAddress from, MailAddress to, string templateID, object data)
		{
			if (m_client == null)
			{
				Logger.Warning($"No {m_sendGridAPIKey.ConfigKey} specified, email send was suppressed.");
				return;
			}
#if DEBUG
			//EmailHistory.Add(msg);
#endif
			if (string.IsNullOrEmpty(m_sendGridAPIKey.Value))
			{
				Logger.Error($"No SendGrid API key was set - suppressing email send");
				return;
			}
			var t = new Task(async () =>
			{
				try
				{
					await m_client.Mail.SendToSingleRecipientAsync(to, from, templateID, data,
						trackOpens: false, trackClicks: false); // Very important!
				}
				catch (Exception e)
				{
					Logger.Exception(e, $"Failed to send email message");
				}
			});
			t.Start();
		}
	}
}
