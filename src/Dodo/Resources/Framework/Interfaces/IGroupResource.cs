using Resources;
using System;

namespace Dodo
{
	public interface IGroupResource : IDodoResource, IPublicResource
	{
		public const string JOIN_GROUP = "join";
		public const string LEAVE_GROUP = "leave";

		bool IsMember(Guid user);
		int MemberCount { get; }
		void Join(AccessContext context);
		void Leave(AccessContext context);
	}
}
