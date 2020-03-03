using Common;
using System;

namespace Dodo.Users
{
	public abstract class ExecutableToken : OneTimeRedeemableToken
	{
		public override void OnRequest(AccessContext context)
		{
			if(IsRedeemed)
			{
				return;
			}
			try
			{
				ExecuteInternal(context);
			}
			catch(Exception e)
			{
				Logger.Exception(e);
			}
			finally
			{
				IsRedeemed = true;
			}
			return;
		}

		protected abstract void ExecuteInternal(AccessContext context);
	}
}
