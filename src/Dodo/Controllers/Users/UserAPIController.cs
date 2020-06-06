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
		protected UserService UserService => new UserService(Context, HttpContext, new UserAuthManager());

		protected override AuthorizationService<User, UserSchema> AuthService => 
			new UserAuthManager() as AuthorizationService<User, UserSchema>;

		[HttpPost(LOGIN)]
		public async Task<IActionResult> Login([FromBody] LoginModel login)
		{
			return (await UserService.Login(login)).ActionResult;
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
	}
}
