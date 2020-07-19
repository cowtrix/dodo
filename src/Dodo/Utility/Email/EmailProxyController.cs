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
			Logger.Debug(debug);
			foreach (var target in targets)
			{
				/*if(Dodo.DodoApp.EmailRedirects != null)
				{
					var redirect = Dodo.DodoApp.EmailRedirects?.SingleOrDefault(r => r?.Email == target);
					if (!string.IsNullOrEmpty(redirect.Email))
					{
						EmailUtility.SendEmail(redirect.Redirect, "", $"noreply@{Dodo.DodoApp.NetConfig.GetHostname()}", "",
							inboundEmail.Subject, inboundEmail.Text, inboundEmail.Html);
						continue;
					}
				}*/
				var text = inboundEmail.Text.Replace(target, "anonymous")
					.Replace(inboundEmail.From.Email, "anonymous");
				var html = inboundEmail.Html.Replace(target, "anonymous")
					.Replace(inboundEmail.From.Email, "anonymous");
				var proxy = EmailProxy.GetProxyFromKey(inboundEmail.From.Email, target);
				if (proxy == null)
				{
					Logger.Warning($"Couldn't resolve {target}");
					continue;
				}
				EmailUtility.SendEmail(proxy.RemoteEmail, "", proxy.ProxyEmail, "", inboundEmail.Subject, text, html);
			}
			return Ok();
		}
	}
}
