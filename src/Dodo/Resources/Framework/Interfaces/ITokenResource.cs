using Resources.Security;
using Resources.Serializers;

namespace Dodo.Users.Tokens
{
	/// <summary>
	/// This is any resource which can contain tokens. This requires a public/private key pair
	/// </summary>
	public interface ITokenResource : IAsymmCapableResource
	{
		TokenCollection TokenCollection { get; }
	}

	public class AsymmCapableResourceWerializer : ResourceReferenceSerializer<IAsymmCapableResource> { }
	public interface IAsymmCapableResource : IDodoResource
	{
		Passphrase PublicKey { get; }
		Passphrase GetPrivateKey(AccessContext context);
	}

}
