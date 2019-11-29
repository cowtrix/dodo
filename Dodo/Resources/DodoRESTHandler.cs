using System;
using System.Collections.Generic;
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

		protected IResourceManager<T> ResourceManager { get { return DodoServer.ResourceManager<T>(); } }

		protected override T GetResource(string url)
		{
			var paramIndex = url.IndexOf("?");
			if(paramIndex > 0)
			{
				url = url.Substring(0, paramIndex);
			}
			return ResourceManager.GetSingle(x => x.ResourceURL == url);
		}

		protected override void DeleteObjectInternal(T target)
		{
			ResourceManager.Delete(target);
		}

		protected override bool URLIsCreation(string url)
		{
			return GetParentFromURL(url) != null;
		}

		/// <summary>
		/// Is this url a valid creation link?
		/// </summary>
		/// <param name="url"></param>
		/// <returns></returns>
		protected GroupResource GetParentFromURL(string url)
		{
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
			context = new ResourceReference<User>(DodoRESTServer.GetRequestOwner(request, out passphrase));
			if (target == null)
			{
				// TODO
				if (request.Method == EHTTPRequestType.POST)
				{
					permissionLevel = EPermissionLevel.OWNER;
				}
				else
				{
					permissionLevel = EPermissionLevel.PUBLIC;
				}
				return true;
			}
			return ResourceManager.IsAuthorised(request, target, out permissionLevel);
		}

	}
}
