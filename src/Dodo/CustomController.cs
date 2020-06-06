using Common;
using System;
using Microsoft.AspNetCore.Mvc;
using Dodo;
using Microsoft.AspNetCore.Mvc.Filters;
using Dodo.Users;
using System.Linq;
using Dodo.Users.Tokens;

namespace Resources
{
	public abstract class CustomController : Controller
	{
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

		public override void OnActionExecuting(ActionExecutingContext actionContext)
		{
			if (Context.User != null)
			{
				foreach (var token in Context.User.TokenCollection.GetAllTokens<IAutoExecuteToken>(Context))
				{
					token.Execute(Context);
				}
			}
			base.OnActionExecuting(actionContext);
		}
	}
}
