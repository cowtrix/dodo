using Dodo;
using Dodo.Analytics;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using System;

/// <summary>
/// Handler for basic CRUD operations, and notifications if they are supported
/// TODO: make notifications have their own thing somehow
/// </summary>
/// <typeparam name="T">Type of the resource</typeparam>
/// <typeparam name="TSchema">Schema type of the resource</typeparam>
public class ResourceServiceBase<T, TSchema> 
	where T : class, IDodoResource
	where TSchema : ResourceSchemaBase
{
	protected AccessContext Context { get; private set; }
	protected HttpContext HttpContext { get; private set; }
	public AuthorizationService<T, TSchema> AuthService { get; private set; }

	public static IResourceManager<T> ResourceManager => ResourceUtility.GetManager<T>();
	public virtual IResourceManager<User> UserManager => ResourceUtility.GetManager<User>();

	/// <summary>
	/// TODO: hook up to dependency injection one day
	/// </summary>
	/// <param name="context">The security and access context object for authorization</param>
	/// <param name="httpContext">The HTTP Context of the request</param>
	/// <param name="authService">The authorization service for this service</param>
	public ResourceServiceBase(AccessContext context, HttpContext httpContext, AuthorizationService<T, TSchema> authService)
	{
		Context = context;
		HttpContext = httpContext;
		AuthService = authService;
	}

	protected virtual IRequestResult VerifyRequest(string id, EHTTPRequestType type, string actionName = null)
	{
		T target;
		if(Guid.TryParse(id, out var guid))
		{
			target = ResourceManager.GetSingle(rsc => rsc.Guid == guid);
		}
		else
		{
			target = ResourceManager.GetSingle(rsc => rsc.Slug == id);
		}
		if (target == null)
		{
			return ResourceRequestError.NotFoundRequest();
		}
		if(actionName == null && type == EHTTPRequestType.GET)
		{
			Analytics.RegisterView(Context, target);
		}		
		if (!Context.Challenge())
		{
			return ResourceRequestError.ForbidRequest();
		}
		return AuthService.IsAuthorised(Context, target, type, actionName);
	}

	protected virtual IRequestResult VerifyRequest(TSchema schema)
	{
		if (!Context.Challenge())
		{
			return ResourceRequestError.ForbidRequest();
		}
		return AuthService.IsAuthorised(Context, schema, EHTTPRequestType.POST);
	}
}
