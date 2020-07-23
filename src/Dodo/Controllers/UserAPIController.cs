using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dodo.Models;
using static UserService;
using Dodo.Users.Tokens;
using System.Linq;
using Dodo.Rebellions;
using System.Collections.Generic;
using System;

namespace Dodo.Users
{
	[SecurityHeaders]
	[ApiController]
	[Route(RootURL)]
	public class UserAPIController : CrudResourceAPIController<User, UserSchema>
	{
		const string MY_REBELLION = "my-rebellion";
		protected UserService UserService => new UserService(Context, HttpContext, new UserAuthService());

		protected override AuthorizationService<User, UserSchema> AuthService => 
			new UserAuthService() as AuthorizationService<User, UserSchema>;

		[HttpPost(LOGIN)]
		public async Task<IActionResult> Login([FromBody] LoginModel login)
		{
			return (await UserService.Login(login)).ActionResult;
		}

		[HttpGet]
		public async Task<IActionResult> GetCurrentUser()
		{
			if(Context.User == null)
			{
				return Forbid();
			}
			return Ok(Context.User.GenerateJsonView(EPermissionLevel.OWNER, Context.User, Context.Passphrase));
		}

		[HttpGet(MY_REBELLION)]
		public async Task<IActionResult> GetMyRebellion()
		{
			if (Context.User == null)
			{
				return Forbid();
			}
			var ret = new Dictionary<Guid, MyRebellionNode>();
			// Here we get a big ol' unsorted list
			foreach (var token in Context.User.TokenCollection.GetAllTokens<IMyRebellionToken>(Context, EPermissionLevel.OWNER, Context.User))
			{
				if(!ret.TryGetValue(token.Reference.Guid, out var node))
				{
					node = new MyRebellionNode(token.Reference);
					ret[token.Reference.Guid] = node;
				}
				if(token is UserAddedAsAdminToken)
				{
					node.Administrator = true;
				}
				else if (token is UserJoinedGroupToken)
				{
					node.Member = true;
				}				
			}
			while(ret.Values.Any(t => !t.Checked))
			{
				// Ok, now it's time to actually build the tree out
				foreach(var node in ret.Values)	// we leave the root nodes alone
				{
					if(node.Reference.Parent != default)
					{
						if (!ret.TryGetValue(node.Reference.Parent, out var parentNode))
						{
							parentNode = new MyRebellionNode(ResourceUtility.GetResourceByGuid(node.Reference.Parent).CreateRef());
							ret[parentNode.Reference.Guid] = parentNode;
						}
						parentNode.Children.Add(node);
					}
					node.Checked = true;
				}
			}
			var tree = ret.Values.Where(n => n.Reference.Parent == default).ToList();
			return Ok(tree.GenerateJsonViewEnumerable(EPermissionLevel.OWNER, Context.User, Context.Passphrase));
		}

		[HttpGet(LOGOUT)]
		public async Task<IActionResult> Logout()
		{
			return (await UserService.Logout()).ActionResult;
		}

		[HttpGet(RESET_PASSWORD)]
		public async Task<IActionResult> RequestPasswordReset(string email)
		{
			return (await UserService.RequestPasswordReset(email)).ActionResult;
		}

		[HttpPost(RESET_PASSWORD)]
		public async Task<IActionResult> ResetPassword(string token, [FromBody]string password)
		{
			return (await UserService.ResetPassword(token, password)).ActionResult;
		}

		[HttpPost(CHANGE_PASSWORD)]
		public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel model)
		{
			return (await UserService.ChangePassword(model)).ActionResult;
		}

		[HttpGet(VERIFY_EMAIL)]
		public async Task<IActionResult> VerifyEmail(string token)
		{
			return (await UserService.VerifyEmail(token)).ActionResult;
		}

		[HttpPost]
		[Route(REGISTER)]
		public async Task<IActionResult> Register([FromBody] UserSchema schema, [FromQuery]string token = null)
		{
			return (await UserService.Register(schema, token)).ActionResult;
		}

		[HttpGet(Dodo.Users.Tokens.INotificationResource.ACTION_NOTIFICATION)]
		public virtual async Task<IActionResult> GetNotifications([FromQuery]int page = 1)
		{
			if(Context.User == null)
			{
				return Forbid();
			}
			return (await PublicService.GetNotifications(Context.User.Guid.ToString(), page)).ActionResult;
		}
	}
}
