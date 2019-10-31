using Dodo.Dodo;
using SimpleHttpServer.Models;
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
		static void Main(string[] args)
		{
			m_restServer = new RESTServer(8080);
		}
	}

	public class Test : RESTHandler
	{
		[Route("Test", @"/test(?:^/)*", EHTTPRequestType.GET)]
		public HttpResponse TestPage(HttpRequest request)
		{
			return new HttpResponse()
			{
				ContentAsUTF8 = "Hello world!",
				StatusCode = "200",
			};
		}
	}
}
