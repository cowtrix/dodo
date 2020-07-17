using Microsoft.AspNetCore.Mvc;

namespace Resources
{
	public class OkRequestResult : IRequestResult
	{
		public bool IsSuccess => true;

		public object Content { get; set; }

		public IActionResult ActionResult
		{
			get
			{
				if(Content == null)
				{
					return new OkResult();
				}
				return new OkObjectResult(Content);
			}
		}

		public OkRequestResult() { }

		public OkRequestResult(object content)
		{
			Content = content;
		}
	}

	public class ActionRequestResult : IRequestResult
	{
		public bool IsSuccess { get; private set; }

		public IActionResult ActionResult { get; private set; }

		public ActionRequestResult() { }

		public ActionRequestResult(IActionResult result, bool isSuccess = true)
		{
			ActionResult = result;
			IsSuccess = true;
		}
	}
}
