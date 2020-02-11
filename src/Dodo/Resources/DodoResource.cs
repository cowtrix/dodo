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
		// TODO get rid of the context, it should be something fed into the actual resource constructor
		[JsonIgnore]
		public AccessContext Context { get; set; }
		internal DodoResourceSchemaBase(AccessContext context, string name) : base(name)
		{
			Context = context;
		}
		public DodoResourceSchemaBase(string name) : base(name)
		{
		}
		public DodoResourceSchemaBase() : base() { }
	}

	public abstract class DodoResource : Resource, IDodoResource
	{
		public DodoResource(DodoResourceSchemaBase schema) : base(schema)
		{
			Creator = new ResourceReference<User>(schema?.Context.User);
		}
		[View(EPermissionLevel.ADMIN)]
		public ResourceReference<User> Creator { get; private set; }
		public abstract bool IsAuthorised(AccessContext context, EHTTPRequestType requestType, out EPermissionLevel permissionLevel);
	}
}
