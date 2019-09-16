using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using XR.Dodo;

namespace DodoTest
{
	public class SymSyncSimulator
	{
		protected HttpClient m_client = new HttpClient();
		protected readonly string m_address;

		protected string m_secret;

		public SymSyncSimulator(string target)
		{
			m_address = target;
			m_secret = DodoServer.SMSGatewaySecret;
		}

		public async Task<string> SendSMS(string from, string message)
		{
			if(!ValidationExtensions.ValidateNumber(ref from))
			{
				return null;
			}
			var msg = new Dictionary<string, string>()
			{
				{ "from", from },
				{ "message", message },
				{ "message_id", "1" },
				{ "sent_to", "+44784651921" },
				{ "secret", m_secret },
				{ "device_id", "1" },
				{ "sent_timestamp", DateTime.UtcNow.ToFileTimeUtc().ToString() },
			};
			return await SendSMS(msg);
		}

		public async Task<string> SendSMS(Dictionary<string, string> values)
		{
			return await POST(m_address, values);
		}

		protected async Task<string> POST(string address, Dictionary<string, string> values)
		{
			var content = new FormUrlEncodedContent(values);
			var response = await m_client.PostAsync(address, content);
			return await response.Content.ReadAsStringAsync();
		}
	}
}
