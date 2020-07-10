using Common.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;
using Resources.Security;

namespace Dodo.Users.Tokens
{
	public class PlainTokenEntry : TokenEntry
	{
		[BsonElement]
		[JsonProperty]
		private string m_token { get; set; }

		public PlainTokenEntry(ITokenResource owner, IToken token) : base(owner, token)
		{
		}

		public override IToken GetToken(Passphrase context) => JsonConvert.DeserializeObject(m_token, JsonExtensions.StorageSettings) as IToken;

		public override void SetData(ITokenResource owner, IToken token)
		{
			m_token = JsonConvert.SerializeObject(token, JsonExtensions.StorageSettings);
		}
	}
}
