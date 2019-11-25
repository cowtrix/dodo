using System;

namespace SimpleHttpServer
{
	public class HttpException : Exception
	{
		public static HttpException UNAUTHORIZED { get { return new HttpException("Unauthorised", 401); } }
		public static HttpException FORBIDDEN { get { return new HttpException("Forbidden", 403); } }
		public static HttpException NOT_FOUND { get { return new HttpException("Resource not found", 404); } }
		public static HttpException CONFLICT { get { return new HttpException("Conflict - resource may already exist", 409); } }
		public static Exception LOGIN { get { return new HttpException("You need to login", 302); } }

		public readonly int ErrorCode;
		public HttpException(string message, int errorCode) : base(message)
		{
			ErrorCode = errorCode;
		}
	}
}
