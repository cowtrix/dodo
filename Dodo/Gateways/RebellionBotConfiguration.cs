using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodo.Gateways
{
	public class RebellionBotConfiguration
	{
		public class TelegramConfiguration
		{
			[View(EPermissionLevel.OWNER)]
			public string BotSecret;
		}

		public class TwilioConfiguration
		{
			[View(EPermissionLevel.OWNER)]
			public string SID = "";
			[View(EPermissionLevel.OWNER)]
			public string AuthToken = "";
		}

		[View(EPermissionLevel.OWNER)]
		public TelegramConfiguration TelegramConfig;
		[View(EPermissionLevel.OWNER)]
		public TwilioConfiguration TwilioConfig;
	}
}
