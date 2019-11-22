using System;
using System.Collections.Generic;
using Common;
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

		protected override bool IsAuthorised(HttpRequest request, out EPermissionLevel visibility, out object context, out string passphrase)
		{
			var target = GetResource(request.Url);
			if(target != null && !(target is T))
			{
				throw HTTPException.CONFLICT;
			}
			context = null;
			passphrase = null;
			if(target == null)
			{
				// TODO
				if (request.Method == EHTTPRequestType.POST)
				{
					visibility = EPermissionLevel.OWNER;
				}
				else
				{
					visibility = EPermissionLevel.PUBLIC;
				}
				return true;
			}
			context = DodoRESTServer.GetRequestOwner(request, out passphrase);
			return ResourceManager.IsAuthorised(request, target, out visibility);
		}

	}
}
