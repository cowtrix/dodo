using Common;
using Common.Security;
using Microsoft.AspNetCore.Mvc;
using Resources;
using Resources.Security;
using System.IO;
using System.Linq;
using System.Threading.Tasks;
using System;
using System.Text;
using Dodo.Users;
using StrongGrid.Models.Webhooks;
using Common.Config;
using Newtonsoft.Json;

namespace Dodo.Utility
{
	[Route("email")]
	public class EmailProxyController : CustomController
	{
		public async Task<IActionResult> ReceiveEmail()
		{
			var parser = new StrongGrid.WebhookParser();
			var inboundEmail = await parser.ParseInboundEmailWebhookAsync(Request.Body);
			var targets = inboundEmail.To
				.Where(m => m.Email.EndsWith(Dodo.DodoApp.NetConfig.GetHostname()))
				.Select(m => m.Email);
			var debug = new StringBuilder($"Received email from {inboundEmail.From.Email}.");
			if (!targets.Any())
			{
				// No valid targets - spam probably?
				// TODO: forward to some dump instead of throwing away maybe?
				Logger.Debug($"{debug} No matching to target found somehow. {string.Join(", ", targets)}");
				return Ok();
			}
			debug.AppendLine($" Targets: {string.Join(", ", targets)}");
			Logger.Debug(debug.ToString());
			foreach (var target in targets)
			{
				// TODO why is the call to EmailRedirects throwing a null exception?
				/*var redirect = Dodo.DodoApp.EmailRedirects?.SingleOrDefault(r => r?.Email == target);
				if (!string.IsNullOrEmpty(redirect.Email))
				{
					EmailUtility.SendEmail(redirect.Redirect, "", $"noreply@{Dodo.DodoApp.NetConfig.GetHostname()}", "",
						inboundEmail.Subject, inboundEmail.Text, inboundEmail.Html);
					continue;
				}*/
				var text = inboundEmail.Text.Replace(target, "anonymous")
					.Replace(inboundEmail.From.Email, "anonymous");
				var html = inboundEmail.Html.Replace(target, "anonymous")
					.Replace(inboundEmail.From.Email, "anonymous");

				EmailProxy.ProxyInformation proxy = null;
				try
				{
					proxy = EmailProxy.GetProxyFromKey(inboundEmail.From.Email, target);
				}
				catch (Exception e)
				{
					Logger.Warning($"New untargeted email: {inboundEmail.From.Email}");
					SaveEmail(inboundEmail);
					continue;
				}
				if (proxy == null)
				{
					Logger.Warning($"Couldn't resolve {target}");
					continue;
				}
				var remoteEmail = proxy.RemoteEmail;
				var hostname = Dodo.DodoApp.NetConfig.GetHostname();
				if (proxy.RemoteEmail.EndsWith(hostname))
				{
					// forward local emails to username
					var username = remoteEmail.Substring(0, remoteEmail.Length - hostname.Length);
					var user = ResourceUtility.GetManager<User>().GetSingle(u => u.Slug == username);
					if (user == null)
					{
						Logger.Error($"Unable to resolve username proxy {username}");
						continue;
					}
					remoteEmail = user.PersonalData.Email;
				}
				EmailUtility.SendEmail(remoteEmail, Dodo.DodoApp.PRODUCT_NAME, proxy.ProxyEmail, Dodo.DodoApp.PRODUCT_NAME,
					inboundEmail.Subject, text, html);

			}
			return Ok();
		}

		private void SaveEmail(InboundEmail inboundEmail)
		{
			var savePath = Path.GetFullPath(ConfigManager.GetValue("EmailSavePath", ".\\email\\"));
			if (!Directory.Exists(savePath))
			{
				Directory.CreateDirectory(savePath);
			}
			System.IO.File.WriteAllText(Path.Combine(savePath, $"{inboundEmail.From.Email}-{DateTime.Now.Ticks}.txt"), 
				JsonConvert.SerializeObject(inboundEmail, Formatting.Indented));
		}
	}
}
