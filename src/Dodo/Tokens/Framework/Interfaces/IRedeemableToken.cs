namespace Dodo.Users.Tokens
{
	/// <summary>
	/// Tokens implementing this interface can be redeemed a single time by the user
	/// with the user's authentication details
	/// </summary>
	public interface IRedeemableToken : IUserToken
	{
		bool IsRedeemed	{ get; }
		bool Redeem(AccessContext context);
	}

	/// <summary>
	/// Tokens implementing this interface will be automtically executed when the user connects
	/// with authentication details
	/// </summary>
	public interface IAutoExecuteToken : IUserToken
	{
		bool HasExecuted { get; }
		bool Execute(AccessContext context);
	}
}
