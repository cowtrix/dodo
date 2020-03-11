namespace Dodo.Users.Tokens
{
	/// <summary>
	/// Tokens implementing this interface can provide a notification to the user
	/// </summary>
	public interface INotificationToken : IUserToken
	{
		string GetNotification(AccessContext context);
	}
}
