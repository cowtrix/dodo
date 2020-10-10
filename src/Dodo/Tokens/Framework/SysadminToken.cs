using Resources;
using System.Collections;
using System.Collections.Generic;

namespace Dodo.Users.Tokens
{
	public class SysadminToken : Token
	{
		public override bool Encrypted => true;
		public Queue<string> CommandHistory { get; set; } = new Queue<string>();

		public override EPermissionLevel GetVisibility() => EPermissionLevel.OWNER;
	}
}
