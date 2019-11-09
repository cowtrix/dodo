using Common;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace SimpleHttpServer
{
	public static class HttpExtensions
	{
		public static bool GetUserPassword(this HttpRequest request, out string username, out string password)
		{
			if (!request.Headers.TryGetValue(RESTServer.TOKEN_KEY, out var token))
			{
				username = null;
				password = null;
				return false;
			}
			var tokens = token.Trim().Split(' ');
			if (tokens.Length != 2 || tokens[0] != "Basic")
			{
				throw HTTPException.UNAUTHORIZED;
			}
			var decode = StringExtensions.Base64Decode(tokens[1]).Split(':');
			username = decode[0];
			password = decode[1];
			return true;
		}
	}
}
