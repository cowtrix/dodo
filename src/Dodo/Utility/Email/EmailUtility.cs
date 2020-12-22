using Common;
using Common.Config;
using Common.Extensions;
using Common.Security;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Resources;
using Resources.Security;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Net;
using System.Text;
using System.Threading.Tasks;

namespace Dodo.Email
{
	public struct EmailAddress
	{
		public string Email;
		public string Name;

		public EmailAddress(string email, string name)
		{
			this.Email = email;
			this.Name = name;
		}

		public string GetToken() => SHA256Utility.SHA256(DodoApp.ServerSalt + Email).Replace("-", "");
	}

	public static class EmailUtility
	{
		public struct EmailConfiguration
		{
			public string FromEmail;
			public string FromName;
			public string SMTPAddress;
			public int SMTPPort;
			public string SMTPUsername;
			public string SMTPPassword;
		}

		private static EmailConfiguration m_emailConfig =
			new ConfigVariable<EmailConfiguration>("Dodo_EmailConfiguration", default).Value;
		private static string[] m_banners;
		private static PersistentStore<string, bool> m_unsubscribed = new PersistentStore<string, bool>(DodoApp.PRODUCT_NAME, "UnsubbedEmails");
		private static Dictionary<string, string> m_templateCache = new Dictionary<string, string>();
		private static SmtpClient SMTPClient;

		static EmailUtility()
		{
			if (string.IsNullOrEmpty(m_emailConfig.SMTPAddress))
			{
				return;
			}
			try
			{
				var webroot = DodoApp.WebRoot;
				m_banners = Directory.GetFiles(Path.Combine(webroot, "img", "email"), "banner*.jpg")
					.Select(s => s.Replace(webroot, DodoApp.NetConfig.FullURI).Replace('\\', '/'))
					.ToArray();

				SMTPClient = new SmtpClient();
				SMTPClient.Connect(m_emailConfig.SMTPAddress, m_emailConfig.SMTPPort, SecureSocketOptions.StartTls);
				SMTPClient.Authenticate(m_emailConfig.SMTPUsername, m_emailConfig.SMTPPassword);
			}
			catch(Exception e)
			{
				Logger.Exception(e, "Exception in EmailUtility");
			}
		}

		static Dictionary<string, string> GetStandardTemplateData(EmailAddress target, string subject) => new Dictionary<string, string>()
		{
			{ "PRODUCT_NAME", DodoApp.PRODUCT_NAME },
			{ "PRIVACY_POLICY", DodoApp.PrivacyPolicyURL },
			{ "PRODUCT_URL", DodoApp.NetConfig.FullURI },
			{ "UNSUBSCRIBE", $"{DodoApp.NetConfig.FullURI}/unsubscribe?token={target.GetToken()}" },
			{ "BANNER", m_banners.Random() },
			{ "SUBJECT", subject }
		};

		static string GetTemplate(string templateName)
		{
			if (m_templateCache.TryGetValue(templateName, out var txt))
			{
				return txt;
			}
			var fPath = Path.GetFullPath($@"EmailTemplates\{templateName}.template.html");
			if (!File.Exists(fPath))
			{
				throw new FileNotFoundException(fPath);
			}
			txt = File.ReadAllText(fPath);
			m_templateCache[templateName] = txt;
			return txt;
		}

		public static void SendEmail(EmailAddress target, string subject, string template, Dictionary<string, string> data)
		{
			if(string.IsNullOrEmpty(m_emailConfig.SMTPAddress))
			{
				return;
			}
			var from = new EmailAddress(m_emailConfig.FromEmail, m_emailConfig.FromName);
			if (m_unsubscribed.ContainsKey(target.Email))
			{
				Logger.Debug($"Email to {target.Email} was suppressed because user unsubscribed");
			}
			var t = new Task(() =>
			{
				try
				{
					var content = GetTemplate(template);
					foreach (var val in GetStandardTemplateData(target, subject))
					{
						content = content.Replace($"{{{val.Key}}}", val.Value);
					}
					foreach (var val in data)
					{
						content = content.Replace($"{{{val.Key}}}", val.Value);
					}
					
				   // create email message
				    var email = new MimeMessage();
					email.From.Add(new MailboxAddress(from.Name, from.Email));
					email.To.Add(new MailboxAddress(target.Name, target.Email));
					email.Subject = subject;
					email.Body = new TextPart(TextFormat.Html) { Text = content };

					// send email
					SMTPClient.Send(email);
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
