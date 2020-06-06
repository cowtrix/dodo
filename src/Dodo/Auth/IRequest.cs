using Microsoft.AspNetCore.Mvc;

namespace Resources
{
	public interface IRequestResult
	{
		bool IsSuccess { get; }
		IActionResult Result { get; }
	}
}
