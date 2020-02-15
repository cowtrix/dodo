using Dodo.Users;
using Dodo.Utility;
using Microsoft.AspNetCore.Http;
using Resources;
using System;
using System.Linq;

namespace DodoResources
{
	[Obsolete]
	public class DodoRESTManager
	{
		private void ProcessPushActions(HttpRequest request)
		{
			// TODO
			/*var context = request.TryGetRequestOwner();
			if (context.User == null)
			{
				return;
			}
			using (var locker = new ResourceLock(context.User))
			{
				var user = locker.Value as User;
				foreach (var pushAction in user.PushActions.Actions.Where(pa => pa.AutoFire))
				{
					pushAction.Execute(context);
					ResourceUtility.GetManager<User>().Update(user, locker);
				}
			}*/
		}
	}
}
