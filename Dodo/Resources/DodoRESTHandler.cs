using Common;
using Dodo.Users;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo
{
	public abstract class DodoRESTHandler<T> : ObjectRESTHandler<T> where T: class, IRESTResource
	{
		const string TOKEN_KEY = "Authorization";

		/// <summary>
		/// Get the user that made an HTTP request.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>The user that made this request</returns>
		protected static User GetRequestOwner(HttpRequest request)
		{
			if(!request.Headers.TryGetValue(TOKEN_KEY, out var token))
			{
				return null;
			}
			var tokens = token.Trim().Split(' ');
			if(tokens.Length !=2 || tokens[0] != "Basic")
			{
				throw HTTPException.UNAUTHORIZED;
			}
			var decode = StringExtensions.Base64Decode(tokens[1]).Split(':');
			var user = DodoServer.UserManager.GetSingle(x => x.WebAuth.Username == decode[0]);
			if(user != null && !user.WebAuth.Challenge(decode[1]))
			{
				throw HTTPException.FORBIDDEN;
			}
			return user;
		}

		protected override bool IsAuthorised(HttpRequest request)
		{
			var requestType = request.Method;
			var target = GetResource(request.Url);
			var owner = GetRequestOwner(request);
			return IsAuthorised(owner, request, target);
		}

		/// <summary>
		/// Is the given user authorized to make the specified request against the target?
		/// </summary>
		/// <param name="owner">The user making the request</param>
		/// <param name="requestType">The type of request</param>
		/// <param name="target">The resource they are targeting</param>
		/// <returns></returns>
		protected abstract bool IsAuthorised(User owner, HttpRequest request, T target);
	}
}
