using Resources;
using Microsoft.AspNetCore.Http;

namespace Dodo
{
	public class GroupResourceService<T, TSchema> : ResourceServiceBase<T, TSchema>
		where T : class, IGroupResource
		where TSchema : DescribedResourceSchemaBase
	{
		public GroupResourceService(AccessContext context, HttpContext httpContext, AuthorizationService<T, TSchema> authService) 
			: base(context, httpContext, authService)
		{
		}

		public IRequestResult JoinGroup(string id)
		{
			var reqResult = VerifyRequest(id, EHTTPRequestType.POST, IGroupResource.JOIN_GROUP);
			if (!reqResult.IsSuccess)
			{
				return reqResult;
			}
			var req = (ResourceActionRequest)reqResult;
			using var resourceLock = new ResourceLock(req.Result);
			var target = resourceLock.Value as T;
			target.Join(req.AccessContext);
			ResourceManager.Update(target, resourceLock);
			return new OkRequestResult();
		}

		public IRequestResult LeaveGroup(string id)
		{
			var reqResult = VerifyRequest(id, EHTTPRequestType.POST, IGroupResource.LEAVE_GROUP);
			if (!reqResult.IsSuccess)
			{
				return reqResult;
			}
			var req = (ResourceActionRequest)reqResult;
			using var resourceLock = new ResourceLock(req.Result);
			var target = resourceLock.Value as T;
			target.Leave(req.AccessContext);
			ResourceManager.Update(target, resourceLock);
			return new OkRequestResult();
		}
	}
}
