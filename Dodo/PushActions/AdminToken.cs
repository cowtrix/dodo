namespace Dodo.Users
{
	[SingletonPushAction]
	public class AdminToken : PushAction
	{
		public override string Message => "";

		public override bool AutoFire => false;
	}
}
