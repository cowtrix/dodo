using Dodo.Users;
using Dodo.Utility;
using Microsoft.AspNetCore.Http;
using REST;
using System;
using System.Linq;

namespace Dodo
{
	public class DodoRESTManager : RESTManager
	{
		public DodoRESTManager() : base()
		{
			AddResourceLookupRoute();
			OnMsgReceieved += ProcessPushActions;
		}

		private void ProcessPushActions(HttpRequest request)
		{
			var owner = request.TryGetRequestOwner(out var passphrase);
			if (owner == null)
			{
				return;
			}
			using (var locker = new ResourceLock(owner))
			{
				foreach (var pushAction in owner.PushActions.Actions.Where(pa => pa.AutoFire))
				{
					pushAction.Execute(owner, passphrase);
					ResourceUtility.GetManager<User>().Update(owner, locker);
				}
			}
		}

		private void AddResourceLookupRoute()
		{
			// Add resource lookup
			Routes.Add(
				new Route("Resource lookup", EHTTPRequestType.GET, (url) => url.StartsWith("resources/"),
					request =>
					{
						return RESTHandler.WrapRawCall(req =>
						{
							if (!Guid.TryParse(request.Path.Value?.Substring("resources/".Length), out var guid))
							{
								throw HttpException.NOT_FOUND;
							}
							var resource = ResourceUtility.GetResourceByGuid(guid) as DodoResource;
							if (resource == null)
							{
								throw HttpException.NOT_FOUND;
							}
							if (!ResourceUtility.IsAuthorized(request, resource, out var view))
							{
								throw HttpException.FORBIDDEN;
							}
							var owner = request.GetRequestOwner(out var passphrase);
							return HttpBuilder.OK(resource.GenerateJsonView(view, owner, passphrase));
						}).Invoke(request);
					}
				)
			);
		}
	}
}
