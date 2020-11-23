using Common;
using Common.Config;
using System;
using System.Collections.Generic;
using System.IO;
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
	}

	public static class EmailUtility
	{
		static ConfigVariable<string> m_emailFrom = new ConfigVariable<string>("Email_FromEmail", $"noreply@{DodoApp.PRODUCT_NAME}.com");
		static ConfigVariable<string> m_nameFrom = new ConfigVariable<string>("Email_FromName", $"{DodoApp.PRODUCT_NAME} SysAdmin");

		static Dictionary<string, string> GetStandardTemplate() => new Dictionary<string, string>()
		{
			{ "product_name", DodoApp.PRODUCT_NAME },
			{ "privacy_policy", DodoApp.PrivacyPolicyURL },
			{ "url", DodoApp.NetConfig.FullURI },
		};

		static EmailUtility()
		{
		}

		public static void SendEmail(EmailAddress target, string subject, string template, Dictionary<string, string> data)
		{
			var from = new EmailAddress(m_emailFrom.Value, m_nameFrom.Value);
			var t = new Task(async () =>
			{
				try
				{
					var fPath = $@"wwwroot\img\email\{template}.template.html";
					if(!File.Exists(fPath))
					{
						throw new FileNotFoundException(fPath);
					}
					//var tmplate = File.
				}
				catch (Exception e)
				{
					Logger.Exception(e, $"Failed to send email message");
				}
			});
			t.Start();
		}

		private static void SendCallbackEmail(EmailAddress to, string subject, string callbackurl, string template)
		{
			var dynamicTemplateData = GetStandardTemplate();
			dynamicTemplateData["NAME"] = to.Name;
			dynamicTemplateData["CALLBACK_URL"] = callbackurl;
			SendEmail(to, subject, template, dynamicTemplateData);
		}
	}
}
