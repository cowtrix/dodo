using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public enum EGatewayType
	{
		SMS,
		Telegram,
	}

	public interface IMessageGateway
	{
		EGatewayType Type { get; }
		void SendMessage(ServerMessage message, UserSession session);
	}
}
