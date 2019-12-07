using Common;
using Common.Extensions;
using Common.Security;
using Dodo.Resources;
using Dodo.Users;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;

namespace Dodo
{
	public class DodoRESTServer : RESTServer
	{
		public DodoRESTServer(int port, string certificate, string sslPassword) : base(port, certificate, sslPassword)
		{
			AddResourceLookupRoute();
			OnMsgReceieved += ProcessPushActions;
		}

		private void ProcessPushActions(HttpRequest request)
		{
			var owner = TryGetRequestOwner(request, out var passphrase);
			if (owner == null)
			{
				return;
			}
			lock (owner.PushActions)
			{
				foreach (var pushAction in owner.PushActions.AllActions.Where(pa => pa.AutoFire))
				{
					pushAction.Execute(owner, passphrase);
				}
			}
		}

		private void AddResourceLookupRoute()
		{
			// Add resource lookup
			Routes.Add(new Route("Resource lookup", EHTTPRequestType.GET, (url) => url.StartsWith("resources/"),
				request =>
				{
					try
					{
						if (!Guid.TryParse(request.Url.Substring("resources/".Length), out var guid))
						{
							throw HttpException.NOT_FOUND;
						}
						var resource = ResourceUtility.GetResourceByGuid(guid) as DodoResource;
						if (resource == null)
						{
							throw HttpException.NOT_FOUND;
						}
						if (!ResourceUtility.IsAuthorized(request, resource, out var view))
						{
							throw HttpException.FORBIDDEN;
						}
						var owner = GetRequestOwner(request, out var passphrase);
						return HttpBuilder.OK(resource.GenerateJsonView(view, owner, passphrase));
					}
					catch (Exception e)
					{
						Logger.Exception(e.InnerException);
						if (e.InnerException is HttpException)
						{
							return HttpBuilder.Custom("Error processing request:\n" + e.InnerException.Message, (e.InnerException as HttpException).ErrorCode);
						}
						return HttpBuilder.ServerError("Error processing request:\n" + e.InnerException.Message);
					}
				}
			));
		}

		public string GetURL()
		{
			return System.Net.Dns.GetHostName();
		}

		/// <summary>
		/// Get the user that made an HTTP request, and validate authentication,
		/// </summary>
		/// <param name="request"></param>
		/// <returns>The user that made this request</returns>
		public static User GetRequestOwner(HttpRequest request)
		{
			return GetRequestOwner(request, out _);
		}

		public static User TryGetRequestOwner(HttpRequest request, out Passphrase passphrase)
		{
			try
			{
				return GetRequestOwner(request, out passphrase);
			}
			catch (HttpException)
			{ }
			return null;
		}

		/// <summary>
		/// Get the user that made an HTTP request, validate authentication, and return the
		/// unlocked passphrase. DO NOT store this passphrase anywhere except as a local
		/// scope variable.
		/// </summary>
		/// <param name="request">The requ</param>
		/// <param name="passphrase"></param>
		/// <returns>The user that made this request</returns>
		public static User GetRequestOwner(HttpRequest request, out Passphrase passphrase)
		{
			if (!request.Headers.TryGetValue(AUTH_KEY, out var token))
			{
				return null;
			}
			var tokens = token.Trim().Split(' ');
			if (tokens.Length != 2 || tokens[0] != "Basic")
			{
				throw HttpException.UNAUTHORIZED;
			}
			var decode = StringExtensions.Base64Decode(tokens[1]).Split(':');
			var user = ResourceUtility.GetManager<User>().GetSingle(x => x.WebAuth.Username == decode[0]);
			if (user != null && !user.WebAuth.Challenge(decode[1], out passphrase))
			{
				throw HttpException.FORBIDDEN;
			}
			return user;
		}
	}
}
