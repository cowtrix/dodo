﻿using Common;
using Dodo.Resources;
using Dodo.Users;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;

namespace Dodo
{
	public class DodoRESTServer : RESTServer
	{
		/// <summary>
		/// Get the user that made an HTTP request.
		/// </summary>
		/// <param name="request"></param>
		/// <returns>The user that made this request</returns>
		public static User GetRequestOwner(HttpRequest request)
		{
			return GetRequestOwner(request, out _);
		}

		public static User GetRequestOwner(HttpRequest request, out string passphrase)
		{
			passphrase = null;
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
			var user = DodoServer.ResourceManager<User>().GetSingle(x => x.WebAuth.Username == decode[0]);
			if (user != null && !user.WebAuth.Challenge(decode[1], out passphrase))
			{
				throw HttpException.FORBIDDEN;
			}
			return user;
		}

		public DodoRESTServer(int port, string certificate, string sslPassword) : base(port, certificate, sslPassword)
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
							return HttpBuilder.Error("Error processing request:\n" + e.InnerException.Message, (e.InnerException as HttpException).ErrorCode);
						}
						return HttpBuilder.Error("Error processing request:\n" + e.InnerException.Message);
					}
				}
			));
		}
	}
}
