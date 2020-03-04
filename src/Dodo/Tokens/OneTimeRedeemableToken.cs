using MongoDB.Bson.Serialization.Attributes;

namespace Dodo.Users
{
	public class OneTimeRedeemableToken : UserToken 
	{
		[BsonElement]
		public bool IsRedeemed { get; set; }
	}
}
