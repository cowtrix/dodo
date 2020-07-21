using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dodo.Models;
using static UserService;

namespace Dodo.Users
{
	[SecurityHeaders]
	[ApiController]
	[Route(RootURL)]
	public class UserAPIController : CrudResourceAPIController<User, UserSchema>
	{
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
