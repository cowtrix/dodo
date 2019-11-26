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
			[View(EUserPriviligeLevel.ADMIN)]
			public string BotSecret;
		}

		public class TwilioConfiguration
		{
			[View(EUserPriviligeLevel.ADMIN)]
			public string SID = "";
			[View(EUserPriviligeLevel.ADMIN)]
			public string AuthToken = "";
		}

		[View(EUserPriviligeLevel.ADMIN)]
		public TelegramConfiguration TelegramConfig;
		[View(EUserPriviligeLevel.ADMIN)]
		public TwilioConfiguration TwilioConfig;
	}
}
