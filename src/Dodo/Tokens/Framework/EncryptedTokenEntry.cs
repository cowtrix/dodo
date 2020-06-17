using MongoDB.Bson.Serialization.Attributes;
using Resources;
using Resources.Security;

namespace Dodo.Users.Tokens
{
	public class EncryptedTokenEntry : TokenEntry
	{
		[BsonElement]
		private AsymmEncryptedStore<IToken> m_data;

		public EncryptedTokenEntry(ITokenResource owner, IToken token, EPermissionLevel permissionLevel = EPermissionLevel.OWNER) : base(owner, token, permissionLevel)
		{
			m_data = new AsymmEncryptedStore<IToken>(token, new Passphrase(owner.PublicKey));
		}

		public override IToken GetToken(Passphrase pk)
		{
			if(pk.Equals(default(Passphrase)))
			{
				return default;
			}
			return m_data.GetValue(pk);
		}
	}
}
