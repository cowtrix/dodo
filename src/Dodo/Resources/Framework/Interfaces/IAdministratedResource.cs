using Resources.Security;
using Dodo.Users;

namespace Dodo
{


	public interface IAdministratedResource : IDodoResource
	{
		bool IsAdmin(User target, AccessContext requesterContext, out AdministratorPermissionSet permissions);
		bool AddNewAdmin(AccessContext context, User newAdmin);
		bool UpdateAdmin(AccessContext context, User newAdmin, AdministratorPermissionSet permissions);
		bool CompleteAdminInvite(AccessContext context, Passphrase newPass);
		bool RemoveAdmin(AccessContext context, User targetUser);
	}
}
