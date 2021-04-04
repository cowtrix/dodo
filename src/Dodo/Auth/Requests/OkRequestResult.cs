using Microsoft.AspNetCore.Mvc;

namespace Resources
{
	public abstract class ActionRequestResult : IRequestResult
	{
		public bool IsSuccess { get; private set; }

		public IActionResult ActionResult => GetResult(Message, IsSuccess);

		public string Message { get; private set; }

		public ActionRequestResult(string message, bool isSuccess = true)
		{
			Message = message;
			IsSuccess = isSuccess;
		}

		protected abstract IActionResult GetResult(string message, bool isSuccess = true);
	}

	public class OkRequestResult : ActionRequestResult
	{
		public object Content { get; set; }

		public OkRequestResult(object content = null) : base(content?.ToString())
		{
			Content = content;
		}

		protected override IActionResult GetResult(string message, bool isSuccess = true)
		{
			if (Content == null)
			{
				return new OkResult();
			}
			return new OkObjectResult(Content);
		}
	}

	public class RedirectRequestResult : ActionRequestResult
	{
		public bool IsPermanent { get; private set; }
		public RedirectRequestResult(string message, bool isPermanent = false) : base(message)
		{
			IsPermanent = isPermanent;
		}

		protected override IActionResult GetResult(string message, bool isSuccess = true)
		{
			return new RedirectResult(message, IsPermanent);
		}
	}

}
