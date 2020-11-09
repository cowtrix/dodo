using Microsoft.AspNetCore.Mvc;
using System;

namespace Resources
{
	public class ResourceRequestError : IRequestResult
	{
		public static IRequestResult Conflict() => new ResourceRequestError(new ConflictResult());
		public static IRequestResult BadRequest() => new ResourceRequestError(new BadRequestResult());
		public static IRequestResult BadRequest(string error = null) => new ResourceRequestError(new BadRequestObjectResult(error), error);
		public static IRequestResult ForbidRequest(string error = null) => new ResourceRequestError(new ForbidResult(error), error);
		public static IRequestResult UnauthorizedRequest(string error = null) => new ResourceRequestError(new UnauthorizedObjectResult(error), error);
		public static IRequestResult NotFoundRequest() => new ResourceRequestError(new NotFoundResult());

		public bool IsSuccess => false;

		public IActionResult ActionResult { get; private set; }
		public virtual string Message { get; private set; }

		public ResourceRequestError(IActionResult result, string error = null)
		{
			ActionResult = result;
			Message = error;
		}
	}
}
