using Dodo.Rebellions;
using Dodo.Users;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodo
{
	internal static class DodoServer
	{
		private static RESTServer m_restServer;
		public static UserManager SessionManager;
		public static RebellionManager RebellionManager;

		static void Main(string[] args)
		{
			SessionManager = new UserManager();
			RebellionManager = new RebellionManager();
			m_restServer = new RESTServer(8080);
		}
	}
}
