using Microsoft.AspNetCore.Mvc;

namespace Resources
{
	public interface IRequestResult
	{
		bool IsSuccess { get; }
		IActionResult ActionResult { get; }
	}
}
