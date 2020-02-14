using REST.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using REST;
using Newtonsoft.Json;

namespace Dodo
{
	public interface IDodoResource : IRESTResource
	{
		ResourceReference<User> Creator { get; }
		bool IsAuthorised(AccessContext context, EHTTPRequestType requestType, out EPermissionLevel permissionLevel);
	}

	public class DodoResourceSchemaBase : ResourceSchemaBase
	{
		public DodoResourceSchemaBase(string name) : base(name)
		{
		}
		public DodoResourceSchemaBase() : base() { }
	}

	public abstract class DodoResource : Resource, IDodoResource
	{
		public DodoResource(AccessContext creator, DodoResourceSchemaBase schema) : base(schema)
		{
			Creator = new ResourceReference<User>(creator.User);
		}
		[View(EPermissionLevel.ADMIN)]
		public ResourceReference<User> Creator { get; private set; }
		public abstract bool IsAuthorised(AccessContext context, EHTTPRequestType requestType, out EPermissionLevel permissionLevel);
	}
}
