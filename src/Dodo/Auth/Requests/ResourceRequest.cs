using Dodo;
using Microsoft.AspNetCore.Mvc;

namespace Resources
{
	public abstract class ResourceRequest : IRequestResult
	{
		public IRESTResource Result { get; set; }

		public readonly AccessContext AccessContext;
		public readonly EHTTPRequestType RequestType;
		public string ActionName;
		public readonly EPermissionLevel PermissionLevel;

		protected ResourceRequest(AccessContext context, EHTTPRequestType type, EPermissionLevel permissionLevel)
		{
			AccessContext = context;
			RequestType = type;
			PermissionLevel = permissionLevel;
		}

		public bool IsSuccess => true;

		public IActionResult ActionResult
		{
			get
			{
				var view = DodoJsonViewUtility.GenerateJsonView(Result, PermissionLevel, AccessContext.User, AccessContext.Passphrase);
				return new OkObjectResult(view);
			}
		}

		public abstract string Message { get; }
	}
}
