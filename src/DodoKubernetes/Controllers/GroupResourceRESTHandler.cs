using System.Collections.Generic;
using Common;
using Common.Extensions;
using REST.Security;
using Dodo.Users;
using Dodo.Utility;
using Newtonsoft.Json;
using REST;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Mvc;
using Common.Security;
using System.Net;

namespace Dodo
{
	public abstract class GroupResourceRESTHandler<T> : DodoRESTHandler<T> where T : GroupResource
	{
		public const string ADD_ADMIN = "?addadmin";
		public const string JOIN_GROUP = "?join";
		public const string LEAVE_GROUP = "?leave";

		public override void AddRoutes(List<Route> routeList)
		{
			routeList.Add(new Route(
				$"{GetType().Name} ADD ADMIN",
				EHTTPRequestType.POST,
				url => IsResourceActionUrl(url, ADD_ADMIN),
				WrapRawCall((req) => AddAdministrator(req))
				));
			routeList.Add(new Route(
				$"{GetType().Name} JOIN",
				EHTTPRequestType.POST,
				url => IsResourceActionUrl(url, JOIN_GROUP),
				WrapRawCall((req) => JoinGroup(req))
				));
			routeList.Add(new Route(
				$"{GetType().Name} LEAVE",
				EHTTPRequestType.POST,
				url => IsResourceActionUrl(url, LEAVE_GROUP),
				WrapRawCall((req) => LeaveGroup(req))
				));
			base.AddRoutes(routeList);
		}

		private bool IsResourceActionUrl(string url, string postfix)
		{
			if (!url.EndsWith(postfix))
			{
				return false;
			}
			url = url.Substring(0, url.Length - postfix.Length);
			var resource = ResourceUtility.GetResourceByURL(url) as GroupResource;
			if (resource == null)
			{
				return false;
			}
			return resource is T;
		}

		IActionResult AddAdministrator(HttpRequest request)
		{
			if (!IsAuthorised(request, out var permissionLevel, out var ownerObj, out var passphrase))
			{
				throw HttpException.NOT_FOUND;
			}
			if (permissionLevel < EPermissionLevel.ADMIN)
			{
				throw HttpException.FORBIDDEN;
			}
			var owner = ((ResourceReference<User>)ownerObj).Value;
			var resourceUrl = request.Path.Value?.Substring(0, request.Path.Value.Length - ADD_ADMIN.Length);
			var resource = ResourceUtility.GetResourceByURL(resourceUrl) as GroupResource;
			using (var rscLock = new ResourceLock(resource))
			{
				if (resource == null)
				{
					throw HttpException.NOT_FOUND;
				}
				var targetEmail = JsonConvert.DeserializeObject<string>(request.ReadBody());
				var userManager = ResourceUtility.GetManager<User>() as UserManager;
				var temporaryPassword = default(Passphrase);
				var targetUser = ResourceUtility.GetManager<User>().GetSingle(x => x.GUID.ToString() == targetEmail || x.Email == targetEmail);
				if (targetUser != null && resource.IsAdmin(targetUser, owner, passphrase))
				{
					return HttpBuilder.OK();
				}

				if (targetUser == null && ValidationExtensions.EmailIsValid(targetEmail))
				{
					// This is an email invite for an unregistered user
					// so we create a temporary user, and send an email to the address
					// with a one-off token
					EmailHelper.SendEmail(targetEmail, null, $"{Dodo.PRODUCT_NAME}: You have been invited to administrate " + resource.Name,
						$"You have been invited to administrate the {resource.Name} {resource.GetType().GetName()}.\n" +
						$"To accept this invitation, register your account at {Dns.GetHostName()}/{UserRESTHandler.CREATION_URL}");
					targetUser = userManager.CreateTemporaryUser(targetEmail, out temporaryPassword);
				}
				else
				{
					temporaryPassword = new Passphrase(KeyGenerator.GetUniqueKey(64));
				}

				using (var userLock = new ResourceLock(targetUser))
				{
					// Now we grant access to the resource with a temporary password, and then
					// encrypt that temporary password with the user's public key.
					// We then add a PushAction which will replace the temp password with the
					// real one the next time the user logs in
					resource.AddAdmin(owner, passphrase, targetUser, temporaryPassword);
					ResourceManager.Update(resource, rscLock);
					targetUser.PushActions.Add(new AddAdminAction(resource, temporaryPassword, targetUser.WebAuth.PublicKey));
					ResourceUtility.GetManager<User>().Update(targetUser, userLock);
					return HttpBuilder.OK();
				}
			}
		}

		IActionResult JoinGroup(HttpRequest request)
		{
			var owner = request.GetRequestOwner(out var passphrase);
			var resourceURL = GetResourceURL(request.Path);
			using (var resourceLock = new ResourceLock(resourceURL))
			{
				var target = resourceLock.Value as GroupResource;
				if (target == null)
				{
					throw HttpException.NOT_FOUND;
				}
				target.Members.Add(owner, passphrase);
				ResourceManager.Update(target, resourceLock);
				return HttpBuilder.OK();
			}
		}

		IActionResult LeaveGroup(HttpRequest request)
		{
			var owner = request.GetRequestOwner(out var passphrase);
			var resourceURL = GetResourceURL(request.Path);
			using (var resourceLock = new ResourceLock(resourceURL))
			{
				var target = resourceLock.Value as GroupResource;
				if (target == null)
				{
					throw HttpException.NOT_FOUND;
				}
				target.Members.Remove(owner, passphrase);
				ResourceManager.Update(target, resourceLock);
				return HttpBuilder.OK();
			}
		}

		protected override bool CanCreateAtUrl(ResourceReference<User> requestOwner, Passphrase passphrase, string url, out string error)
		{
			if (!requestOwner.HasValue)
			{
				error = "You need to login";
				return false;
			}
			if (!requestOwner.Value.EmailVerified)
			{
				error = "You need to verify your email";
				return false;
			}
			error = null;
			return true;
		}
	}
}
