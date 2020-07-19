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
				.Where(m => Dodo.DodoApp.NetConfig.Domains.Any(d => m.Email.EndsWith(d)))
				.Select(m => m.Email);
			var debug = new StringBuilder($"Received email from {inboundEmail.From}.");
			if(!targets.Any())
			{
				// No valid targets - spam probably?
				// TODO: forward to some dump instead of throwing away maybe?
				Logger.Debug($"{debug} No matching to target found somehow?");
				return Ok();
			}
			debug.AppendLine($" Targets: {string.Join(", ", targets)}");
			var redirect = Dodo.DodoApp.EmailRedirects.SingleOrDefault(r => r.Email == inboundEmail.From.Email);
			if (!string.IsNullOrEmpty(redirect.Email))
			{
				EmailUtility.SendEmail(redirect.Redirect, "", $"noreply@{Dodo.DodoApp.NetConfig.Domains.First()}", "", 
					inboundEmail.Subject, inboundEmail.Text, inboundEmail.Html);
			}
			foreach(var target in targets)
			{
				var proxy = EmailProxy.GetProxyFromKey(inboundEmail.From.Email, target);
				if(proxy == null)
				{
					debug.AppendLine("Couldn't resolve!");
					continue;
				}
				EmailUtility.SendEmail(proxy.RemoteEmail, "", proxy.ProxyEmail, "", inboundEmail.Subject, inboundEmail.Text, inboundEmail.Html);
			}
			return Ok();
		}
	}
}
