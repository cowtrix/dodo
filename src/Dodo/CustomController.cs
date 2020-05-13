using Common;
using System;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using Microsoft.AspNetCore.Mvc.Filters;
using Dodo.Users;
using System.Linq;
using Dodo.Users.Tokens;

namespace Resources
{
	public abstract class CustomController<T, TSchema> : Controller
		where T : class, IDodoResource
		where TSchema : ResourceSchemaBase
	{
		protected DodoUserManager UserManager => ResourceUtility.GetManager<User>() as DodoUserManager;
		protected virtual AuthorizationService<T, TSchema> AuthManager => new AuthorizationService<T, TSchema>();

		protected IResourceManager<T> ResourceManager { get { return ResourceUtility.GetManager<T>(); } }

		protected AccessContext Context { 
			get 
			{
				if(__context == null)
				{
					__context = User.GetContext();
				}
				return __context.Value;
			} 
		}
		private AccessContext? __context = null;

		protected ResourceRequest VerifySearchRequest()
		{
			if (!Context.Challenge())
			{
				return ResourceRequest.ForbidRequest;
			}
			if (Context.User == null)
			{
				return new ResourceRequest(Context, null, EHTTPRequestType.GET, EPermissionLevel.PUBLIC);
			}
			return new ResourceRequest(Context, null, EHTTPRequestType.GET, EPermissionLevel.MEMBER);
		}

		protected ResourceRequest VerifyRequest(Guid id, string actionName = null)
		{
			var target = ResourceManager.GetSingle(rsc => rsc.Guid == id);
			if (target == null)
			{
				return ResourceRequest.NotFoundRequest;
			}
			if (!Context.Challenge())
			{
				return ResourceRequest.ForbidRequest;
			}
			return AuthManager.IsAuthorised(Context, target, Request.MethodEnum(), actionName);
		}

		protected ResourceRequest VerifyRequest(TSchema schema)
		{
			if (!Context.Challenge())
			{
				return ResourceRequest.ForbidRequest;
			}
			return AuthManager.IsAuthorised(Context, schema, Request.MethodEnum());
		}

		public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			if (Context.User != null)
			{
				foreach (var token in Context.User.TokenCollection.GetAllTokens<IAutoExecuteToken>(Context))
				{
					token.Execute(Context);
				}
			}
			base.OnActionExecuting(actionContext);
		}
	}
}
