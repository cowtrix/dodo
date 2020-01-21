using REST.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using REST;

namespace Dodo
{
	public interface IDodoResource : IRESTResource
	{
		ResourceReference<User> Creator { get; }
		bool IsAuthorised(AccessContext context, EHTTPRequestType requestType, out EPermissionLevel permissionLevel);
	}

	public class DodoResourceSchemaBase : ResourceSchemaBase
	{
		public AccessContext Context { get; set; }
		public DodoResourceSchemaBase(AccessContext context, string name) : base(name)
		{
			Context = context;
		}

		public DodoResourceSchemaBase() : base() { }
	}

	public abstract class DodoResource : Resource, IDodoResource
	{
		public DodoResource(DodoResourceSchemaBase schema) : base(schema)
		{
			Creator = new ResourceReference<User>(schema.Context.User);
		}
		public ResourceReference<User> Creator { get; private set; }
		public abstract bool IsAuthorised(AccessContext context, EHTTPRequestType requestType, out EPermissionLevel permissionLevel);
	}
}
