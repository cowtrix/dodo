using System;
using System.Net;

namespace REST
{
	public class HttpException : Exception
	{
		public static HttpException FORBIDDEN { get { return new HttpException("Forbidden", HttpStatusCode.Forbidden); } }
		public static HttpException NOT_FOUND { get { return new HttpException("Resource not found", HttpStatusCode.NotFound); } }
		public static HttpException CONFLICT { get { return new HttpException("Conflict - resource may already exist", HttpStatusCode.Conflict); } }
		public static HttpException LOGIN { get { return new HttpException("You need to login", HttpStatusCode.Unauthorized); } }
		public static HttpException BAD_REQUEST { get { return new HttpException("Bad request", HttpStatusCode.BadRequest); } }
		public static HttpException UNAUTHORIZED { get { return new HttpException("Unauthorized", HttpStatusCode.Unauthorized); } }

		public readonly HttpStatusCode ErrorCode;
		public HttpException(string message, HttpStatusCode errorCode) : base(message)
		{
			ErrorCode = errorCode;
		}

	}
}
