using Common;
using System;

namespace Dodo.Users
{
	public abstract class ExecutableToken : OneTimeRedeemableToken
	{
		public bool Execute(AccessContext context)
		{
			if(IsRedeemed)
			{
				return false;
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
			return true;
		}

		protected abstract void ExecuteInternal(AccessContext context);
	}
}
