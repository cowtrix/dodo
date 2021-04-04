using Microsoft.AspNetCore.Mvc;

namespace Resources
{
	public interface IRequestResult
	{
		public string Message { get; }
		bool IsSuccess { get; }
		IActionResult ActionResult { get; }
	}
}
