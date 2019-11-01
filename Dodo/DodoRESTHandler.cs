using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo.Users
{
	public abstract class DodoRESTHandler<T> : ObjectRESTHandler<T> where T: class, IRESTResource
	{
		const string USERNAME_KEY = "user";
		const string TOKEN_KEY = "token";

		/// <summary>
		/// Get the user that made an HTTP request.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>The user that made this request</returns>
		protected static User GetRequestOwner(HttpRequest request)
		{
			if(!request.Headers.TryGetValue(USERNAME_KEY, out var username) || !request.Headers.TryGetValue(TOKEN_KEY, out var token))
			{
				return null;
			}
			var user = DodoServer.SessionManager.GetUserByUsername(username);
			if(user != null && user.IsValidToken(token))
			{
				return user;
			}
			return null;
		}
	}
}
