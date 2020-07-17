using Resources.Security;

namespace Dodo.Users.Tokens
{
	/// <summary>
	/// This is any resource which can contain tokens. This requires a public/private key pair
	/// </summary>
	public interface ITokenResource : IDodoResource
	{
		string PublicKey { get; }
		Passphrase GetPrivateKey(AccessContext context);
		TokenCollection TokenCollection { get; }
	}

}
