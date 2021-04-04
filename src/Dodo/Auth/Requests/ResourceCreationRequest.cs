using Common.Extensions;
using Dodo;
using Dodo.Users.Tokens;
using System;

namespace Resources
{
	public class ResourceCreationRequest : ResourceRequest, ICreationContext
	{
		public readonly Guid Token;
		public readonly ResourceSchemaBase Schema;

		public ResourceCreationRequest(AccessContext context,
			ResourceSchemaBase schema,
			ResourceCreationToken token = null)
			: base (context, EHTTPRequestType.POST, EPermissionLevel.ADMIN)
		{
			Schema = schema;
			if(token != null)
			{
				Token = token.Guid;
			}
		}

		public override string Message => $"Successfully created new resource [{Schema.Name}]";

		public bool CanVerify() => true;

		public ResourceSchemaBase GetSchema() => Schema;

		public bool VerifyExplicit(out string error) => Schema.Verify(out error);
	}
}
