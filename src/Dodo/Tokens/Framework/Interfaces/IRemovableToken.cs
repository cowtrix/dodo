namespace Dodo.Users.Tokens
{
	public interface IRemovableToken : IUserToken
	{
		bool CanRemove { get; }
		void OnRemove(User parent);
	}
}
