using Microsoft.AspNetCore.Mvc;
using Dodo;
using Dodo.Users;
using Dodo.Users.Tokens;

namespace Resources
{

	public class ResourceRequest
	{
		public static ResourceRequest BadRequest => new ResourceRequest(new BadRequestResult());
		public static ResourceRequest ForbidRequest => new ResourceRequest(new ForbidResult());
		public static ResourceRequest UnauthorizedRequest => new ResourceRequest(new UnauthorizedResult());
		public static ResourceRequest NotFoundRequest => new ResourceRequest(new NotFoundResult());

		public readonly IDodoResource Resource;
		public readonly ResourceSchemaBase Schema;
		public readonly AccessContext Requester;
		public readonly EHTTPRequestType RequestType;
		public readonly EPermissionLevel PermissionLevel;
		public readonly bool IsSuccess;
		public readonly IActionResult Error;
		public readonly ResourceCreationToken Token;

		private ResourceRequest(NotFoundResult notFoundResult)
		{
			IsSuccess = false;
			Error = notFoundResult;
		}

		private ResourceRequest(ForbidResult forbid)
		{
			IsSuccess = false;
			Error = forbid;
		}

		private ResourceRequest(UnauthorizedResult unauthorizedResult)
		{
			IsSuccess = false;
			Error = unauthorizedResult;
		}

		private ResourceRequest(BadRequestResult badRequestResult)
		{
			IsSuccess = false;
			Error = badRequestResult;
		}

		public ResourceRequest(AccessContext context, IDodoResource rsc, EHTTPRequestType type, EPermissionLevel permissionLevel)
		{
			Requester = context;
			Resource = rsc;
			RequestType = type;
			PermissionLevel = permissionLevel;
			IsSuccess = true;
			Error = null;
		}

		public ResourceRequest(AccessContext context, ResourceSchemaBase schema, EHTTPRequestType type, 
			EPermissionLevel permissionLevel, ResourceCreationToken token = null)
		{
			Requester = context;
			Schema = schema;
			RequestType = type;
			PermissionLevel = permissionLevel;
			IsSuccess = true;
			Error = null;
			Token = token;
		}
	}
}
