namespace Dodo.Users.Tokens
{
	public interface IRemovableToken : IToken
	{
		bool ShouldRemove { get; }
		void OnRemove(AccessContext parent);
	}
}
