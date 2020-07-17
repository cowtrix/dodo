using Dodo;

namespace Resources
{
	public class ResourceActionRequest : ResourceRequest
	{
		public readonly string ActionName;

		public ResourceActionRequest(AccessContext context,
			IDodoResource rsc,
			EHTTPRequestType type,
			EPermissionLevel permissionLevel,
			string actionName = null)
			: base (context, type, permissionLevel)
		{
			Result = rsc;
			ActionName = actionName;
		}
	}
}
