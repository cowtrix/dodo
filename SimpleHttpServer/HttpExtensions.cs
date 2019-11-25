using Common;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace SimpleHttpServer
{
	public static class HttpExtensions
	{
		/// <summary>
		/// Decode the username and password embedded by Basic Authentication in the headers
		/// </summary>
		/// <param name="request"></param>
		/// <param name="username"></param>
		/// <param name="password"></param>
		/// <returns></returns>
		public static bool GetUserPassword(this HttpRequest request, out string username, out string password)
		{
			if (!request.Headers.TryGetValue(RESTServer.AUTH_KEY, out var token))
			{
				username = null;
				password = null;
				return false;
			}
			var tokens = token.Trim().Split(' ');
			if (tokens.Length != 2 || tokens[0] != "Basic")
			{
				throw HttpException.UNAUTHORIZED;
			}
			var decode = StringExtensions.Base64Decode(tokens[1]).Split(':');
			username = decode[0];
			password = decode[1];
			return true;
		}
	}
}
