namespace Dodo.Users.Tokens
{
	/// <summary>
	/// Tokens that implement this interface expire, which means they will be deleted
	/// when IsExpired returns true
	/// </summary>
	public interface IExpiringToken : IUserToken
	{
		bool IsExpired { get; }
	}
}
