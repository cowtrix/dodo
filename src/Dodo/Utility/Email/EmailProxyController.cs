using Common;
using Microsoft.AspNetCore.Mvc;
using Resources;
using Resources.Security;
using System.Linq;
using System.Threading.Tasks;

namespace Dodo.Utility
{
	public static class EmailProxy
	{
		//Dictionary<Guid, MultiSigEncryptedStore<>>
	}

	[Route("email")]
	public class EmailProxyController : CustomController
	{
		public async Task<IActionResult> ReceiveEmail()
		{
			var parser = new StrongGrid.WebhookParser();
			var inboundEmail = await parser.ParseInboundEmailWebhookAsync(Request.Body);
			Logger.Debug($"Received email {inboundEmail}");
			var targets = inboundEmail.To.Where(m => Dodo.DodoApp.NetConfig.Domains.Any(d => m.Email.EndsWith(d)));
			if(!targets.Any())
			{
				// No valid targets - spam probably?
				// TODO: forward to some dump instead of throwing away maybe?
				return Ok();
			}

			return Ok();
		}
	}
}
