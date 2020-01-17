using Dodo.Users;
using Dodo.Utility;
using Microsoft.AspNetCore.Http;
using REST;
using System;
using System.Linq;

namespace Dodo
{
	[Obsolete]
	public class DodoRESTManager : RESTManager
	{
		public DodoRESTManager() : base()
		{
			AddResourceLookupRoute();
			OnMsgReceieved += ProcessPushActions;
		}

		private void ProcessPushActions(HttpRequest request)
		{
			var context = request.TryGetRequestOwner();
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
			}
		}
	}
}
