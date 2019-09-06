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
		protected string m_number = "44712345678";

		public SymSyncSimulator(string target)
		{
			m_address = target;
		}

		public async Task<string> SendSMS(string message)
		{
			var msg = new Dictionary<string, string>()
			{
				{ "from", m_number },
				{ "message", message },
				{ "message_id", "1" },
				{ "sent_to", "011595154631" },
				{ "secret", DodoServer.SECRET },
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
