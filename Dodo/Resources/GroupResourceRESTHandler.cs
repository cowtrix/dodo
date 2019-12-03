using System;
using System.Collections.Generic;
using Common;
using Common.Extensions;
using Common.Security;
using Dodo.Users;
using Dodo.Utility;
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

		/// <summary>
		/// This will add an adminstrator to this resource
		/// </summary>
		/// <param name="request"></param>
		/// <returns></returns>
		HttpResponse AddAdmin(HttpRequest request)
		{
			if (!IsAuthorised(request, out var permissionLevel, out var ownerObj, out var passphrase))
			{
				throw HttpException.NOT_FOUND;
			}
			if(permissionLevel < EPermissionLevel.ADMIN)
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
			var target = JsonConvert.DeserializeObject<string>(request.Content);
			var targetUser = ResourceUtility.GetManager<User>().GetSingle(x => x.GUID.ToString() == target || x.Email == target);
			if(targetUser != null && resource.IsAdmin(targetUser, owner, passphrase))
			{
				return HttpBuilder.OK();
			}
			if(targetUser == null && ValidationExtensions.EmailIsValid(target))
			{
				// This is an email invite for an unregistered user
				// so we create a temporary user, and send an email to the address
				// with a one-off token
				var userManager = ResourceUtility.GetManager<User>() as UserManager;
				targetUser = userManager.CreateTemporaryUser(target);
				EmailHelper.SendEmail(target, null, $"Dodo: You have been invited to administrate " + resource.Name,
					$"You have been invited to administrate the {resource.Name} {resource.GetType().GetName()}.\n" +
					$"To accept this invitation, register your account at {DodoServer.GetURL()}/{UserRESTHandler.CREATION_URL}");
			}
			// Now we grant access to the resource with a temporary password, and then
			// encrypt that temporary password with the user's public key.
			// We then add a PushAction which will replace the temp password with the
			// real one the next time the user logs in
			var temporaryPassword = new Passphrase(KeyGenerator.GetUniqueKey(64));
			resource.AddAdmin(owner, passphrase, targetUser, temporaryPassword);
			targetUser.PushActions.Add(new AddAdminAction(resource, temporaryPassword, targetUser.WebAuth.PublicKey));
			return HttpBuilder.OK();
		}
	}
}
