using Common;
using MongoDB.Bson.Serialization.Attributes;

namespace Dodo.Users.Tokens
{

	public abstract class RedeemableToken : Token, IRedeemableToken, IRemovableToken
	{
		[BsonElement]
		public bool IsRedeemed { get; private set; }

		public bool ShouldRemove => IsRedeemed;

		/// <summary>
		/// Redeem this token.
		/// </summary>
		/// <param name="context">The context of the requester redeeming it</param>
		/// <returns>True if the token was redeemed successfully</returns>
		public bool Redeem(AccessContext context)
		{
			if (IsRedeemed)
			{
				Logger.Warning($"Attempting to redeem already redeemed token: {Guid}");
				return false;
			}
			if(OnRedeemed(context))
			{
				IsRedeemed = true;
				return true;
			}
			return false;
		}

		protected virtual bool OnRedeemed(AccessContext context) => true;

		public virtual void OnRemove(AccessContext parent)
		{
		}
	}
}
