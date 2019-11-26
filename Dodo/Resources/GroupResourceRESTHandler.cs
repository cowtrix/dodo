using System;
using System.Collections.Generic;
using Common;
using Dodo.Users;
using Newtonsoft.Json;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo
{
	public abstract class GroupResourceRESTHandler<T> : DodoRESTHandler<T> where T:GroupResource
	{
		public const string ADD_ADMIN = "?addadmin";
		public override void AddRoutes(List<Route> routeList)
		{
			routeList.Add(new Route(
				$"{GetType().Name} ADD ADMIN",
				EHTTPRequestType.POST,
				IsAddAdminURL,
				WrapRawCall((req) => AddAdmin(req))
				));
			base.AddRoutes(routeList);
		}

		private bool IsAddAdminURL(string url)
		{
			if(!url.EndsWith(ADD_ADMIN))
			{
				return false;
			}
			url = url.Substring(0, url.Length - ADD_ADMIN.Length);
			var resource = ResourceUtility.GetResourceByURL(url) as GroupResource;
			if(resource == null)
			{
				return false;
			}
			return resource is T;
		}

		HttpResponse AddAdmin(HttpRequest request)
		{
			if (!IsAuthorised(request, out var permissionLevel, out var ownerObj, out var passphrase))
			{
				throw HttpException.NOT_FOUND;
			}
			if(permissionLevel < EUserPriviligeLevel.ADMIN)
			{
				throw HttpException.FORBIDDEN;
			}
			var owner = ((ResourceReference<User>)ownerObj).Value;
			var resourceUrl = request.Url.Substring(0, request.Url.Length - ADD_ADMIN.Length);
			var resource = ResourceUtility.GetResourceByURL(resourceUrl) as GroupResource;
			if(resource == null)
			{
				throw HttpException.NOT_FOUND;
			}
			var targetUserGUID = Guid.Parse(JsonConvert.DeserializeObject<string>(request.Content));
			var targetUser = DodoServer.ResourceManager<User>().GetSingle(x => x.GUID == targetUserGUID);
			var temporaryPassword = KeyGenerator.GetUniqueKey(64);
			resource.AddAdmin(owner, passphrase, targetUser, temporaryPassword);
			targetUser.PushActions.Add(new AddAdminAction(resource, temporaryPassword, targetUser.WebAuth.PublicKey));
			return HttpBuilder.OK();
		}
	}
}
