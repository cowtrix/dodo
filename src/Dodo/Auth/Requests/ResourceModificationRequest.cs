using Dodo;

namespace Resources
{
	public class ResourceActionRequest : ResourceRequest
	{
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

		public override string Message => $"Executed action \"{ActionName}\" against {Result}";
	}
}
