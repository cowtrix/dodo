using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Net.Http;
using System.Threading.Tasks;
using XR.Dodo;

namespace DodoTest
{
	public class TwilioSimulator
	{
		protected HttpClient m_client = new HttpClient();
		protected readonly string m_address;

		public TwilioSimulator(string target)
		{
			m_address = target;
		}

		public async Task<string> SendSMS(string from, string message)
		{
			if(!ValidationExtensions.ValidateNumber(ref from))
			{
				return null;
			}
			var msg = new Dictionary<string, string>()
			{
				{ "ToCountry", "GB" },
				{ "ToState", "" },
				{ "SmsMessageSid", "" },
				{ "NumMedia", "0" },
				{ "ToCity", "" },
				{ "FromZip", "" },
				{ "SmsSid", "" },
				{ "FromState", "" },
				{ "SmsStatus", "" },
				{ "FromCity", "" },
				{ "Body", message },
				{ "FromCountry", "GB" },
				{ "To", DodoServer.SMSGateway.GetPhone().Number.ToString() },
				{ "ToZip", "" },
				{ "NumSegments", "1" },
				{ "MessageSid", "" },
				{ "AccountSid", DodoServer.Configuration.GatewayData.TwilioSID },
				{ "From", from },
				{ "ApiVersion", "2010-04-01" },
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
