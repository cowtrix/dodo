using Common;
using System;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using Microsoft.AspNetCore.Mvc.Filters;
using Dodo.Users;

namespace Resources
{
	public class CustomController<T, TSchema> : Controller
		where T : class, IDodoResource
		where TSchema : DodoResourceSchemaBase
	{
		protected DodoUserManager UserManager => ResourceUtility.GetManager<User>() as DodoUserManager;
		protected virtual AuthorizationManager<T, TSchema> AuthManager =>
			new AuthorizationManager<T, TSchema>(this.ControllerContext, Request);

		protected IResourceManager<T> ResourceManager { get { return ResourceUtility.GetManager<T>(); } }


		protected void LogRequest()
		{
			Logger.Debug($"Received {Request.MethodEnum()} for {Request.Path}.");
		}

		protected ResourceRequest VerifyRequest(Guid id = default)
		{
			LogRequest();
			var target = ResourceManager.GetSingle(rsc => rsc.GUID == id);
			var context = User.GetContext();
			if (!context.Challenge())
			{
				return ResourceRequest.ForbidRequest;
			}
			return AuthManager.IsAuthorised(context, target, Request.MethodEnum());
		}

		protected ResourceRequest VerifyRequest(TSchema schema)
		{
			LogRequest();
			var context = User.GetContext();
			if (!context.Challenge())
			{
				return ResourceRequest.ForbidRequest;
			}
			return AuthManager.IsAuthorised(context, schema, Request.MethodEnum());
		}

		public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			var context = User.GetContext();
			if (context.User != null)
			{
				foreach (var token in context.User.TokenCollection.Tokens)
				{
					token.OnRequest(context);
				}
			}
			base.OnActionExecuting(actionContext);
		}
	}
}
