using Dodo;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using System;

public class ResourceServiceBase<T, TSchema> 
	where T : DodoResource
	where TSchema : ResourceSchemaBase
{
	protected AccessContext Context { get; private set; }
	protected HttpContext HttpContext { get; private set; }
	public AuthorizationService<T, TSchema> AuthService { get; private set; }

	protected virtual IResourceManager<T> ResourceManager => ResourceUtility.GetManager<T>();
	protected virtual IResourceManager<User> UserManager => ResourceUtility.GetManager<User>();
	public ResourceServiceBase(AccessContext context, HttpContext httpContext, AuthorizationService<T, TSchema> authService)
	{
		Context = context;
		HttpContext = httpContext;
		AuthService = authService;
	}

	protected IRequestResult VerifyRequest(Guid id, EHTTPRequestType type, string actionName = null)
	{
		var target = ResourceManager.GetSingle(rsc => rsc.Guid == id);
		if (target == null)
		{
			return ResourceRequestError.NotFoundRequest();
		}
		if (!Context.Challenge())
		{
			return ResourceRequestError.ForbidRequest();
		}
		return AuthService.IsAuthorised(Context, target, type, actionName);
	}

	protected IRequestResult VerifyRequest(TSchema schema)
	{
		if (!Context.Challenge())
		{
			return ResourceRequestError.ForbidRequest();
		}
		return AuthService.IsAuthorised(Context, schema, EHTTPRequestType.POST);
	}
}
