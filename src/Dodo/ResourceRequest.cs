using Microsoft.AspNetCore.Mvc;
using Dodo;

namespace Resources
{
	public class ResourceRequest
	{
		public AccessContext Requester;
		public IRESTResource Resource;
		public EHTTPRequestType RequestType;
		public EPermissionLevel PermissionLevel;
		public bool IsSuccess;
		public IActionResult Error;

		public ResourceRequest(NotFoundResult notFoundResult)
		{
			IsSuccess = false;
			Error = notFoundResult;
		}

		public ResourceRequest(ForbidResult forbid)
		{
			IsSuccess = false;
			Error = forbid;
		}

		public ResourceRequest(UnauthorizedResult unauthorizedResult)
		{
			IsSuccess = false;
			Error = unauthorizedResult;
		}

		public ResourceRequest(BadRequestResult badRequestResult)
		{
			IsSuccess = false;
			Error = badRequestResult;
		}

		public ResourceRequest(AccessContext context, IRESTResource rsc, EHTTPRequestType type, EPermissionLevel permissionLevel)
		{
			Requester = context;
			Resource = rsc;
			RequestType = type;
			PermissionLevel = permissionLevel;
			IsSuccess = true;
			Error = null;
		}
	}
}
