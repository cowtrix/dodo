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
			[View(EViewVisibility.OWNER)]
			public string BotSecret;
		}

		public class TwilioConfiguration
		{
			[View(EViewVisibility.OWNER)]
			public string SID = "";
			[View(EViewVisibility.OWNER)]
			public string AuthToken = "";
		}

		[View(EViewVisibility.OWNER)]
		public TelegramConfiguration TelegramConfig;
		[View(EViewVisibility.OWNER)]
		public TwilioConfiguration TwilioConfig;
	}
}
