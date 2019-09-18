using System;
using System.Collections.Generic;
using System.Net;
using System.Threading;
using System.Threading.Tasks;
using Twilio;
using Twilio.Rest.Api.V2010.Account;

namespace XR.Dodo
{
	public class TwilioGateway : IMessageGateway
	{
		private string m_originNumber;

		public TwilioGateway(string accountSid, string authToken, string originNumber)
		{
			TwilioClient.Init(accountSid, authToken);
			ServicePointManager.SecurityProtocol = SecurityProtocolType.Tls
												| SecurityProtocolType.Tls11
												| SecurityProtocolType.Tls12
												| SecurityProtocolType.Ssl3;
			m_originNumber = originNumber;
		}

		public EGatewayType Type { get { return EGatewayType.Twilio; } }

		public void SendMessage(ServerMessage message, UserSession session)
		{
			var msgResource = MessageResource.Create(
				body: message.Content,
				from: new Twilio.Types.PhoneNumber(m_originNumber),
				to: new Twilio.Types.PhoneNumber("+" + session.GetUser().PhoneNumber)
			);
		}

		public void SendMessage(ServerMessage message, string phoneNumber)
		{
			var msgResource = MessageResource.Create(
				body: message.Content,
				from: new Twilio.Types.PhoneNumber(m_originNumber),
				to: new Twilio.Types.PhoneNumber("+" + phoneNumber)
			);
		}
	}
}
