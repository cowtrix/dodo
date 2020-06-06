using Microsoft.AspNetCore.Mvc;

namespace Resources
{
	public class OkRequestResult : IRequestResult
	{
		public bool IsSuccess => true;

		public object Content { get; set; }

		public IActionResult Result
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
}
