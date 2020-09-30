using Common;
using Common.Config;
using Common.Extensions;
using Dodo;
using Dodo.Models;
using Dodo.Users;
using Dodo.Users.Tokens;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Resources;
using Resources.Security;
using System;
using System.Linq;
using System.Threading.Tasks;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

public class CrudResourceServiceBase<T, TSchema> : ResourceServiceBase<T, TSchema>
	where T : class, IDodoResource
	where TSchema : ResourceSchemaBase
{
	public CrudResourceServiceBase(AccessContext context, HttpContext httpContext, AuthorizationService<T, TSchema> auth) 
		: base(context, httpContext, auth)
	{
	}

	public virtual async Task<IRequestResult> Create(TSchema schema)
	{
		var request = VerifyRequest(schema);
		if (!request.IsSuccess)
		{
			return request;
		}
		var req = (ResourceCreationRequest)request;
		var factory = ResourceUtility.GetFactory<T>();
		T createdObject;
		try
		{
			createdObject = factory.CreateObject(req) as T;
			if (req.Token != default && req.AccessContext.User != null)
			{
				// The user consumed a resource creation token to make this resource
				using (var rscLock = new ResourceLock(req.AccessContext.User))
				{
					var user = rscLock.Value as User;
					var token = user.TokenCollection.GetToken<ResourceCreationToken>(Context, req.Token);
					if (token == null)
					{
						throw new Exception("Resource creation token was missing");
					}
					if (token.IsRedeemed)
					{
						SecurityWatcher.RegisterEvent(Context.User, $"Unexpected token consumption could indicate a user " +
							"is attempting to exploit creation of multiple resources.");
						return ResourceRequestError.BadRequest();
					}
					token.Redeem(Context);
					token.Target = createdObject.CreateRef<IRESTResource>(); ;
					UserManager.Update(user, rscLock);
				}
			}
		}
		catch (Exception e)
		{
			return ResourceRequestError.BadRequest($"Failed to deserialise JSON: {e.Message}");
		}
		req.Result = createdObject;
		return req;
	}

	public virtual async Task<IRequestResult> Update(string id, object values)
	{
		var request = VerifyRequest(id, EHTTPRequestType.PATCH);
		if (!request.IsSuccess)
		{
			return request;
		}
		try
		{
			var req = (ResourceActionRequest)request;
			T target;
			using (var resourceLock = new ResourceLock(req.Result))
			{
				target = resourceLock.Value as T;
				if (target == null)
				{
					return ResourceRequestError.NotFoundRequest();
				}
				var jsonSettings = new JsonSerializerSettings()
				{
					TypeNameHandling = TypeNameHandling.All
				};
				target.PatchObject(values, req.PermissionLevel, req.AccessContext.User, req.AccessContext.Passphrase);
				ResourceManager.Update(target, resourceLock);
			}
			req.Result = target;
			return req;
		}
		catch(PublicException e)
		{
			return ResourceRequestError.BadRequest(e.Message);
		}
	}

	public virtual async Task<IRequestResult> Delete(string id)
	{
		var request = VerifyRequest(id, EHTTPRequestType.DELETE);
		if (!request.IsSuccess)
		{
			return request;
		}
		var req = (ResourceActionRequest)request;
		ResourceManager.Delete(req.Result as T);
		return new OkRequestResult("Resource deleted");
	}

	public virtual async Task<IRequestResult> Get(string id)
	{
		return VerifyRequest(id, EHTTPRequestType.GET);
	}

	public virtual async Task<IRequestResult> GetNotifications(string id, int page)
	{
		int chunk = ConfigManager.GetValue($"Notifications_ChunkSize", 5);
		if (!typeof(INotificationResource).IsAssignableFrom(typeof(T)))
		{
			return ResourceRequestError.BadRequest("This resource type does not support notifications");
		}
		var request = VerifyRequest(id, EHTTPRequestType.GET, INotificationResource.ACTION_NOTIFICATION);
		if (!request.IsSuccess)
		{
			return request;
		}
		if(!(request is ResourceActionRequest actionReq))
		{
			Logger.Error($"Request was unexpectedly {request.GetType().Name}");
			return ResourceRequestError.BadRequest();
		}
		var notificationProvider = actionReq.Result as INotificationResource;
		var notifications = notificationProvider.GetNotifications(actionReq.AccessContext, actionReq.PermissionLevel);
		var notificationChunk = new
		{
			notifications = notifications.Skip((page - 1) * chunk).Take(chunk),
			totalCount = notifications.Count(),
			chunkSize = chunk,
		};
		return new OkRequestResult(notificationChunk);
	}

	public virtual async Task<IRequestResult> AddNotification(string id, NotificationModel notification)
	{
		if (!typeof(INotificationResource).IsAssignableFrom(typeof(T)))
		{
			return ResourceRequestError.BadRequest("This resource type does not support notifications");
		}
		var request = VerifyRequest(id, EHTTPRequestType.POST, INotificationResource.ACTION_NOTIFICATION);
		if (!request.IsSuccess)
		{
			return request;
		}
		var req = (ResourceActionRequest)request;
		T target;
		using (var resourceLock = new ResourceLock(req.Result))
		{
			target = resourceLock.Value as T;
			var notificationRsc = target as INotificationResource;
			if (target == null)
			{
				return ResourceRequestError.NotFoundRequest();
			}
			var notificationToken = new SimpleNotificationToken(Context.User, null, notification.Message, null, ENotificationType.Announcement, notification.PermissionLevel, true);
			notificationRsc.TokenCollection.AddOrUpdate(notificationRsc, notificationToken);
			ResourceManager.Update(target, resourceLock);
		}
		req.Result = target;
		return req;
	}

	public virtual async Task<IRequestResult> DeleteNotification(string id, Guid notification)
	{
		if (!typeof(INotificationResource).IsAssignableFrom(typeof(T)))
		{
			return ResourceRequestError.BadRequest("This resource type does not support notifications");
		}
		var request = VerifyRequest(id, EHTTPRequestType.POST, INotificationResource.ACTION_NOTIFICATION);
		if (!request.IsSuccess)
		{
			return request;
		}
		var req = (ResourceActionRequest)request;
		INotificationResource target;
		using (var resourceLock = new ResourceLock(req.Result))
		{
			target = (INotificationResource)resourceLock.Value;
			if(!target.DeleteNotification(req.AccessContext, req.PermissionLevel, notification))
			{
				return ResourceRequestError.NotFoundRequest();
			}
			ResourceManager.Update(target, resourceLock);
		}
		return new OkRequestResult($"Notification {notification} was deleted.");
	}
}
