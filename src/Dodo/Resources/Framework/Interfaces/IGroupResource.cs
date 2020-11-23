using Dodo.Users;
using Resources;

namespace Dodo
{
	public interface IGroupResource : IDodoResource, IPublicResource
	{
		public const string JOIN_GROUP = "join";
		public const string LEAVE_GROUP = "leave";

		bool IsMember(User user);
		int MemberCount { get; }
		void Join(AccessContext context);
		void Leave(AccessContext context);
	}
}
