using MongoDB.Bson.Serialization.Attributes;
using Resources;
using Resources.Security;

namespace Dodo.Users.Tokens
{
	public class EncryptedTokenEntry : TokenEntry
	{
		[BsonElement]
		private AsymmEncryptedStore<Token> m_data;

		public EncryptedTokenEntry(ITokenOwner owner, Token token, EPermissionLevel permissionLevel = EPermissionLevel.OWNER) : base(owner, token, permissionLevel)
		{
			m_data = new AsymmEncryptedStore<Token>(token, new Passphrase(owner.PublicKey));
		}

		public override Token GetToken(Passphrase pk)
		{
			return m_data.GetValue(pk);
		}
	}
}
