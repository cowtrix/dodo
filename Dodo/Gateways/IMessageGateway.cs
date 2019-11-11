using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer.Models;
using System.Collections.Generic;
using System.ComponentModel;
using System.Threading.Tasks;

namespace Dodo
{
	public interface IMessageGateway
	{
		void SendMessage(ServerMessage message, User user);
		void SendMessage(ServerMessage message, IEnumerable<User> users);
	}
}
