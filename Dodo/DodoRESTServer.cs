﻿using Common;
using Dodo.Resources;
using Dodo.Users;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;

namespace Dodo
{
	public class DodoRESTServer : RESTServer
	{
		const string TOKEN_KEY = "Authorization";

		/// <summary>
		/// Get the user that made an HTTP request.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>The user that made this request</returns>
		public static User GetRequestOwner(HttpRequest request)
		{
			if (!request.Headers.TryGetValue(TOKEN_KEY, out var token))
			{
				return null;
			}
			var tokens = token.Trim().Split(' ');
			if (tokens.Length != 2 || tokens[0] != "Basic")
			{
				throw HTTPException.UNAUTHORIZED;
			}
			var decode = StringExtensions.Base64Decode(tokens[1]).Split(':');
			var user = DodoServer.ResourceManager<User>().GetSingle(x => x.WebAuth.Username == decode[0]);
			if (user != null && !user.WebAuth.Challenge(decode[1]))
			{
				throw HTTPException.FORBIDDEN;
			}
			return user;
		}

		public DodoRESTServer(int port) : base(port)
		{
			Routes.Add(new Route()
			{
				Name = "Resource lookup",
				Method = EHTTPRequestType.GET,
				UrlRegex = "resources/(?:^/)*",
				Callable = request =>
				{
					try
					{
						if (!Guid.TryParse(request.Url.Substring("/resources/".Length), out var guid))
						{
							throw HTTPException.NOT_FOUND;
						}
						var resource = ResourceUtility.GetResourceByGuid(guid) as DodoResource;
						if (resource == null)
						{
							throw HTTPException.NOT_FOUND;
						}
						if (!ResourceUtility.IsAuthorized(request, resource, out var view))
						{
							throw HTTPException.FORBIDDEN;
						}
						var owner = GetRequestOwner(request);
						return HttpBuilder.OK(resource.GenerateJsonView(view));
					}
					catch (Exception e)
					{
						Logger.Exception(e.InnerException);
						if (e.InnerException is HTTPException)
						{
							return HttpBuilder.Error("Error processing request:\n" + e.InnerException.Message, (e.InnerException as HTTPException).ErrorCode);
						}
						return HttpBuilder.Error("Error processing request:\n" + e.InnerException.Message);
					}
				},
			});
		}
	}
}