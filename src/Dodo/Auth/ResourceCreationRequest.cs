using Dodo;
using Dodo.Users.Tokens;

namespace Resources
{
	public class ResourceCreationRequest : ResourceRequest
	{
		public readonly ResourceCreationToken Token;
		public readonly ResourceSchemaBase Schema;

		public ResourceCreationRequest(AccessContext context,
			ResourceSchemaBase schema,
			EHTTPRequestType type,
			EPermissionLevel permissionLevel,
			ResourceCreationToken token = null)
			: base (context, type, permissionLevel)
		{
			Schema = schema;
			Token = token;
		}
	}
}
