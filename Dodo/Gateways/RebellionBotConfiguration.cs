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
			[View(EPermissionLevel.ADMIN)]
			public string BotSecret;
		}

		public class TwilioConfiguration
		{
			[View(EPermissionLevel.ADMIN)]
			public string SID = "";
			[View(EPermissionLevel.ADMIN)]
			public string AuthToken = "";
		}

		[View(EPermissionLevel.ADMIN)]
		public TelegramConfiguration TelegramConfig;
		[View(EPermissionLevel.ADMIN)]
		public TwilioConfiguration TwilioConfig;
	}
}
