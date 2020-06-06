using Microsoft.AspNetCore.Mvc;
using System;

namespace Resources
{
	public class ResourceRequestError : IRequestResult
	{
		public static IRequestResult Conflict() => new ResourceRequestError(new ConflictResult());
		public static IRequestResult BadRequest() => new ResourceRequestError(new BadRequestResult());
		public static IRequestResult BadRequest(string error = null) => new ResourceRequestError(new BadRequestObjectResult(error));
		public static IRequestResult ForbidRequest(string error = null) => new ResourceRequestError(new ForbidResult(error));
		public static IRequestResult UnauthorizedRequest() => new ResourceRequestError(new UnauthorizedResult());
		public static IRequestResult NotFoundRequest() => new ResourceRequestError(new NotFoundResult());

		public bool IsSuccess => false;

		public IActionResult Result { get; private set; }

		public ResourceRequestError(IActionResult result)
		{
			Result = result;
		}
	}
}
