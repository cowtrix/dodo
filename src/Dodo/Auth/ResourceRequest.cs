using Dodo;
using Dodo.Users;
using Microsoft.AspNetCore.Mvc;

namespace Resources
{
	public abstract class ResourceRequest : IRequestResult
	{
		public IRESTResource Result { get; set; }

		public readonly AccessContext AccessContext;
		public readonly EHTTPRequestType RequestType;
		public readonly string ActionName;
		public readonly EPermissionLevel PermissionLevel;

		protected ResourceRequest(AccessContext context, EHTTPRequestType type, EPermissionLevel permissionLevel)
		{
			AccessContext = context;
			RequestType = type;
			PermissionLevel = permissionLevel;
		}

		public bool IsSuccess => true;

		public IActionResult ActionResult => 
			new OkObjectResult(DodoJsonViewUtility.GenerateJsonView(Result, PermissionLevel, AccessContext.User, AccessContext.Passphrase));
	}
}