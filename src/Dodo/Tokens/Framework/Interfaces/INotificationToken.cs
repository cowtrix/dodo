namespace Dodo.Users.Tokens
{
	/// <summary>
	/// Tokens implementing this interface can provide a notification to the user
	/// </summary>
	public interface INotificationToken : IToken
	{
		Notification GetNotification(AccessContext context);
	}
}
