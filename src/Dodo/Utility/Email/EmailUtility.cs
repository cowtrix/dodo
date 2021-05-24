using Common;
using Common.Config;
using Common.Extensions;
using Common.Security;
using MailKit.Net.Smtp;
using MailKit.Security;
using MimeKit;
using MimeKit.Text;
using Resources;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
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
		private static PersistentStore<string, bool> m_unsubscribed = new PersistentStore<string, bool>($"{DodoApp.PRODUCT_NAME.Replace(".", "_")}_email", "UnsubbedEmails");
		private static Dictionary<string, string> m_templateCache = new Dictionary<string, string>();
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
			}
			catch(Exception e)
			{
				Logger.Exception(e, "Exception in EmailUtility");
			}
		}

		static Dictionary<string, string> GetStandardTemplateData(EmailAddress target, string subject) => new Dictionary<string, string>()
		{
			{ "PRODUCT_NAME", DodoApp.PRODUCT_NAME },
			{ "PRIVACY_POLICY", $"{DodoApp.NetConfig.FullURI}/{DodoApp.PrivacyPolicyURL}" },
			{ "PRODUCT_URL", DodoApp.NetConfig.FullURI },
			{ "UNSUBSCRIBE", $"{DodoApp.NetConfig.FullURI}/unsubscribe?email={Uri.EscapeUriString(target.Email)}&token={Uri.EscapeUriString(GetEmailHash(target.Email))}" },
			{ "BANNER", m_banners.Random() },
			{ "SUBJECT", subject }
		};

		static string GetTemplate(string templateName)
		{
			if (m_templateCache.TryGetValue(templateName, out var txt))
			{
				return txt;
			}
			var fPath = Path.GetFullPath($@"Content\EmailTemplates\{templateName}.template.html");
			if (!File.Exists(fPath))
			{
				throw new FileNotFoundException(fPath);
			}
			txt = File.ReadAllText(fPath);
			m_templateCache[templateName] = txt;
			return txt;
		}

		public static bool EmailIsUnsubscribed(string email)
		{
			var hash = GetEmailHash(email);
			return HashIsUnsubscribed(hash);
		}

		public static bool HashIsUnsubscribed(string hash)
		{
			return m_unsubscribed.ContainsKey(hash);
		}

		public static void UnsubscribeHash(string hash)
		{
			m_unsubscribed[hash] = true;
		}

		public static void ClearEmailUnsubscription(string email)
		{
			m_unsubscribed.Remove(GetEmailHash(email));
		}

		public static string GetEmailHash(string email)
		{
			return SHA256Utility.SHA256(email + DodoApp.ServerSalt);
		}

		public static void SendEmail(EmailAddress target, string subject, string template, Dictionary<string, string> data)
		{
			if(string.IsNullOrEmpty(m_emailConfig.SMTPAddress))
			{
				return;
			}
			var from = new EmailAddress(m_emailConfig.FromEmail, m_emailConfig.FromName);
			if (EmailIsUnsubscribed(target.Email))
			{
				Logger.Debug($"Email to {target.Email} was suppressed because user unsubscribed");
				return;
			}
			if(target.Email.EndsWith("@example.com"))
			{
				Logger.Debug($"Email to {target.Email} was suppressed because user had fake/test email.");
				return;
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
					using var client = new SmtpClient();
					client.Connect(m_emailConfig.SMTPAddress, m_emailConfig.SMTPPort, SecureSocketOptions.StartTls);
					client.Authenticate(m_emailConfig.SMTPUsername, m_emailConfig.SMTPPassword);
					client.Send(email);
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
