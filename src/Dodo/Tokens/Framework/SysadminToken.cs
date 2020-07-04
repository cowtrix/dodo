using Resources;

namespace Dodo.Users.Tokens
{
	public class SysadminToken : Token
	{
		public override bool Encrypted => true;

		public override EPermissionLevel GetVisibility() => EPermissionLevel.OWNER;
	}
}
