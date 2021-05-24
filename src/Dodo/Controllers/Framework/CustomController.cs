using System;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using Microsoft.AspNetCore.Mvc.Filters;
using System.Linq;
using Dodo.Users.Tokens;

namespace Resources
{
	public abstract class CustomController : Controller
	{
		public const string RESPONSE_VIEWDATA = "server_response";

		/// <summary>
		/// The AccessContext related to this request.
		/// </summary>
		protected AccessContext Context { 
			get 
			{
				if(__context == null)
				{
					__context = User.GetContext();
				}
				return __context.Value;
			} 
		}
		private AccessContext? __context = null;

		/// <summary>
		/// This will execute before any request is processed.
		/// The ViewData is only relevant to Razor views.
		/// </summary>
		/// <param name="actionContext">The context of the executing action.</param>
		public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			ViewData["Timezone"] = Context.User != null && !string.IsNullOrEmpty(Context.User.PersonalData.TimezoneID) 
				? TimeZoneConverter.TZConvert.GetTimeZoneInfo(Context.User.PersonalData.TimezoneID) : TimeZoneInfo.Utc;
			ViewData["Context"] = Context;
			ViewData["header"] = !Request.Query.TryGetValue("header", out var headerVal) || headerVal.Single() == "true";
			if (Context.User != null)
			{
				foreach (var token in Context.User.TokenCollection.GetAllTokens<IAutoExecuteToken>(Context, EPermissionLevel.ADMIN, Context.User))
				{
					token.Execute(Context);
				}
			}
			base.OnActionExecuting(actionContext);
		}
	}
}
