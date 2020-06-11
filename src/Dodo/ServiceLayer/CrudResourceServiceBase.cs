using Dodo;
using Dodo.Users;
using Dodo.Users.Tokens;
using Microsoft.AspNetCore.Http;
using Newtonsoft.Json;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using System.Threading.Tasks;

public class CrudResourceServiceBase<T, TSchema> : ResourceServiceBase<T, TSchema>
	where T : DodoResource
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
						throw new SecurityException($"Unexpected token consumption could indicate a user " +
							"is attempting to exploit creation of multiple resources.");
					}
					token.Redeem(Context);
					token.Target = new ResourceReference<IRESTResource>(createdObject);
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
}
