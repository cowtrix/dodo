using Resources.Security;
using Resources;
using Newtonsoft.Json;
using Common.Extensions;

namespace Dodo
{
	public interface IDodoResource : IRESTResource
	{
		bool IsCreator(AccessContext context);
	}

	public class NotNulResourceAttribute : VerifyMemberBase
	{
		public override bool Verify(object value, out string validationError)
		{
			if (value is IResourceReference rscRef && rscRef.HasValue)
			{
				validationError = null;
				return true;
			}
			validationError = $"Resource reference as null for {value}";
			return false;
		}
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
