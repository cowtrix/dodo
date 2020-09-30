using MongoDB.Bson.Serialization.Attributes;
using Resources;
using Resources.Security;

namespace Dodo.Users.Tokens
{
	public class EncryptedTokenEntry : TokenEntry
	{
		[BsonElement]
		private AsymmEncryptedStore<IToken> m_data;

		public EncryptedTokenEntry(ITokenResource owner, IToken token) : base(owner, token)
		{
		}

		public override IToken GetToken(Passphrase pk)
		{
			if(pk.Equals(default(Passphrase)))
			{
				return default;
			}
			return m_data.GetValue(pk);
		}

		public override void SetData(ITokenResource owner, IToken token)
		{
			m_data = new AsymmEncryptedStore<IToken>(token, owner.PublicKey);
		}
	}
}
