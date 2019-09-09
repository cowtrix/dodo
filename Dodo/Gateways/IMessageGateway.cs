using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace XR.Dodo
{
	public interface IMessageGateway
	{
		void SendMessage(ServerMessage message, UserSession session);
	}
}
