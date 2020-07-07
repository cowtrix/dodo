using Dodo.Users;
using Resources;

namespace Dodo
{
	public class AdministratorEntry
	{
		public AdministratorEntry() { }
		public AdministratorEntry(User user)
		{
			User = user.CreateRef();
		}
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public ResourceReference<User> User { get; set; }
		[View(EPermissionLevel.ADMIN, EPermissionLevel.SYSTEM)]
		public AdministratorPermissionSet Permissions { get; set; } = new AdministratorPermissionSet();
	}
}
