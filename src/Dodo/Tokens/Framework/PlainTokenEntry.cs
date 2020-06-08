using Common.Extensions;
using MongoDB.Bson.Serialization.Attributes;
using Newtonsoft.Json;
using Resources;

namespace Dodo.Users.Tokens
{
	public class PlainTokenEntry : TokenEntry
	{
		[BsonElement]
		[JsonProperty]
		private string m_token { get; set; }

		public PlainTokenEntry(ITokenOwner owner, Token token, EPermissionLevel permissionLevel = EPermissionLevel.OWNER) : base(owner, token, permissionLevel)
		{
			m_token = JsonConvert.SerializeObject(token, JsonExtensions.StorageSettings);
		}

		public override Token GetToken(AccessContext context) => JsonConvert.DeserializeObject(m_token, JsonExtensions.StorageSettings) as Token;
	}
}
