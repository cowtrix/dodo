using Common;
using MongoDB.Bson.Serialization.Attributes;

namespace Dodo.Users.Tokens
{
	public abstract class AutoExecutableToken : UserToken, IAutoExecuteToken, IRemovableToken
	{
		[BsonElement]
		public bool HasExecuted { get; private set; }

		public bool CanRemove => HasExecuted;

		/// <summary>
		/// Redeem this token.
		/// </summary>
		/// <param name="context">The context of the requester redeeming it</param>
		/// <returns>True if the token was redeemed successfully</returns>
		public bool Execute(AccessContext context)
		{
			if (HasExecuted)
			{
				Logger.Warning($"Attempting to execute already redeemed token: {GUID}");
				return false;
			}
			if (OnExecuted(context))
			{
				HasExecuted = true;
				return true;
			}
			return false;
		}

		protected virtual bool OnExecuted(AccessContext context) => true;

		public virtual void OnRemove(User parent)
		{
		}
	}
}
