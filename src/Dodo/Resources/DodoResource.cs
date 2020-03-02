using Resources.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Http;
using Resources;
using Newtonsoft.Json;

namespace Dodo
{
	public interface IDodoResource : IRESTResource
	{
		bool IsCreator(AccessContext context);
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
			if (creator.User != null)
			{
				Creator = SecurityExtensions.GenerateID(creator.User, creator.Passphrase);
			}
		}
		[View(EPermissionLevel.ADMIN)]
		public string Creator { get; private set; }
		public bool IsCreator(AccessContext context)
		{
			return Creator == SecurityExtensions.GenerateID(context.User, context.Passphrase);
		}
	}
}
