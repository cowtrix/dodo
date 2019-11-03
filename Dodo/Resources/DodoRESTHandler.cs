using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo
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
			var user = DodoServer.SessionManager.GetSingle(x => x.WebAuth.Username == username);
			if(user != null && user.IsValidToken(token))
			{
				return user;
			}
			return null;
		}

		protected override bool IsAuthorised(HttpRequest request)
		{
			var requestType = request.Method;
			var target = GetResource(request.Url);
			var owner = GetRequestOwner(request);
			return IsAuthorised(owner, request.Method, target);
		}

		/// <summary>
		/// Is the given user authorized to make the specified request against the target?
		/// </summary>
		/// <param name="user">The user making the request</param>
		/// <param name="requestType">The type of request</param>
		/// <param name="target">The resource they are targeting</param>
		/// <returns></returns>
		protected abstract bool IsAuthorised(User user, EHTTPRequestType requestType, T target);

	}
}
