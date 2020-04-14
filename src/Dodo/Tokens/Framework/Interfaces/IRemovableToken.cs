namespace Dodo.Users.Tokens
{
	public interface IRemovableToken : IUserToken
	{
		bool ShouldRemove { get; }
		void OnRemove(User parent);
	}
}
