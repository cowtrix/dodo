using System;

namespace Dodo.Dodo
{
	[AttributeUsage(AttributeTargets.Method)]
	public class RouteAttribute : Attribute
	{
		public readonly string Name;
		public readonly string URLRegex;
		public readonly EHTTPRequestType RequestType;
		public RouteAttribute(string name, string urlRegex, EHTTPRequestType requestType)
		{
			Name = name;
			URLRegex = urlRegex;
			RequestType = requestType;
		}
	}
}
