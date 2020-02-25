using Dodo.Users;
using Resources;

namespace Dodo
{
	public class AuthorizationManager<T> where T: IDodoResource
	{
		public bool IsAuthorised(AccessContext Context, T target, EHTTPRequestType requestType, out EPermissionLevel permissionLevel)
		{
			if (target != null && !(target is T))
			{
				permissionLevel = EPermissionLevel.PUBLIC;
				return false;
			}
			if (target != null && Context.User == null)
			{
				permissionLevel = EPermissionLevel.PUBLIC;
				if (requestType != EHTTPRequestType.GET)
				{
					return false; // Deny if not logged in and trying to do more than just fetch
				}
				return true; // If it's just GET then return a public view
			}
			if (target == null)
			{
				if (Context.User == null)
				{
					if (typeof(T) == typeof(User) && requestType == EHTTPRequestType.POST)
					{
						// Special case, unregistered requesters can create new users
						permissionLevel = EPermissionLevel.OWNER;  // User is owner
						return true;
					}
					else if (requestType != EHTTPRequestType.GET)
					{
						permissionLevel = EPermissionLevel.PUBLIC;  // Requester not logged in, they can't make or patch stuff
						return false;
					}
					permissionLevel = EPermissionLevel.PUBLIC;
					return true;
				}
				else if (requestType == EHTTPRequestType.POST)
				{
					permissionLevel = EPermissionLevel.OWNER;
					return true;
				}
				permissionLevel = EPermissionLevel.PUBLIC;
				return true;
			}
			return target.IsAuthorised(Context, requestType, out permissionLevel);
		}
	}
}
