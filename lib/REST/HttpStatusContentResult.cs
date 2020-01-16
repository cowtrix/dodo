using Common.Extensions;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Mvc;
using System.Net;

namespace REST
{
	public class HttpStatusContentResult : ActionResult
	{
		private HttpStatusCode m_statusCode;
		private string m_reasonPhrase;

		public HttpStatusContentResult(HttpStatusCode statusCode = HttpStatusCode.OK, string statusDescription = null)
		{
			m_statusCode = statusCode;
			m_reasonPhrase = statusDescription;
		}

		public override void ExecuteResult(ActionContext context)
		{
			var response = context.HttpContext.Response;
			response.StatusCode = (int)m_statusCode;
			if (m_reasonPhrase != null)
			{
				var responseFeature = context.HttpContext.Features.Get<IHttpResponseFeature>();
				if (responseFeature != null)
				{
					responseFeature.ReasonPhrase = m_reasonPhrase;
				}
			}
		}
	}
}
