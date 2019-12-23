using System;
using System.Collections.Generic;
using System.Threading;
using System.Threading.Tasks;
using Common;
using Common.Security;
using Dodo.Resources;
using Dodo.Users;
using SimpleHttpServer;
using SimpleHttpServer.Models;
using SimpleHttpServer.REST;

namespace Dodo
{
	public abstract class DodoRESTHandler<T> : ObjectRESTHandler<T> where T: DodoResource, IRESTResource
	{
		protected virtual string CreationPostfix { get; }

		protected override T GetResource(string url)
		{
			return ResourceManager.GetSingle(x => x.ResourceURL.Equals(GetResourceURL(url)));
		}

		protected override string GetResourceURL(string url)
		{
			var paramIndex = url.IndexOf("?");
			if (paramIndex > 0)
			{
				url = url.Substring(0, paramIndex);
			}
			return url;
		}

		protected override void DeleteObjectInternal(T target)
		{
			ResourceManager.Delete(target);
		}

		protected override bool URLIsCreation(string url)
		{
			if(!url.EndsWith(CreationPostfix))
			{
				return false;
			}
			return GetParentFromURL(url) != null;
		}

		/// <summary>
		/// Is this url a valid creation link?
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		public GroupResource GetParentFromURL(string url)
		{
			if(string.IsNullOrEmpty(url))
			{
				return null;
			}
			if(url.EndsWith(CreationPostfix))
			{
				// Strip creation postfix
				url = url.Substring(0, url.Length - CreationPostfix.Length);
			}
			url = url.TrimEnd('/');
			var resource = ResourceUtility.GetResourceByURL(url) as GroupResource;
			if (resource == null)
			{
				return null;
			}
			if(!resource.CanContain(typeof(T)))
			{
				return null;
			}
			return resource;
		}

		protected override bool IsAuthorised(HttpRequest request, out EPermissionLevel permissionLevel, out object context, out Passphrase passphrase)
		{
			var target = GetResource(request.Url);
			if(target != null && !(target is T))
			{
				throw HttpException.CONFLICT;
			}
			var requestOwner = new ResourceReference<User>(DodoRESTServer.GetRequestOwner(request, out passphrase));
			context = requestOwner;
			if (target != null && requestOwner == null)
			{
				permissionLevel = EPermissionLevel.PUBLIC;
				if (request.Method != EHTTPRequestType.GET)
				{
					return false; // Deny if not logged in and trying to do more than just fetch
				}
				return true; // If it's just GET then return a public view
			}
			if (target == null)
			{
				if (!requestOwner.HasValue)
				{
					if (typeof(T) == typeof(User) && request.Method == SimpleHttpServer.EHTTPRequestType.POST)
					{
						// Special case, unregistered requesters can create new users
						permissionLevel = EPermissionLevel.OWNER;  // User is
						return true;
					}
					else if (request.Method != SimpleHttpServer.EHTTPRequestType.GET)
					{
						permissionLevel = EPermissionLevel.PUBLIC;  // Requester not logged in, they can't make or patch stuff
						return false;
					}
					permissionLevel = EPermissionLevel.PUBLIC;
					return true;
				}
				else if (request.Method == SimpleHttpServer.EHTTPRequestType.POST)
				{
					permissionLevel = EPermissionLevel.OWNER;
					if(!CanCreateAtUrl(requestOwner, passphrase, request.Url, out var error))
					{
						throw new HttpException(error, 500);
					}
					return true;
				}
				permissionLevel = EPermissionLevel.PUBLIC;
				return true;
			}
			return ResourceManager.IsAuthorised(request, target, out permissionLevel);
		}

		protected virtual bool CanCreateAtUrl(ResourceReference<User> requestOwner, Passphrase passphrase, string url, out string error)
		{
			var parent = GetParentFromURL(url);
			if (parent == null)
			{
				error = "Resource not found";
				return false;
			}
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
			return parent.IsAdmin(requestOwner, requestOwner, passphrase);
		}
	}
}
