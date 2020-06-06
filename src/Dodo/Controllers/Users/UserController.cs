using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using Dodo.Models;

namespace Dodo.Users
{
	[SecurityHeaders]
	[ApiController]
	[Route(RootURL)]
	public class UserController : UserControllerBase
	{
		[HttpPost(LOGIN)]
		public async Task<IActionResult> Login([FromBody] LoginModel login)
		{
			return await Login_Internal(login);
		}

		[HttpGet(LOGOUT)]
		public async Task<IActionResult> Logout()
		{
			return await Logout_Internal();
		}

		[HttpGet(RESET_PASSWORD)]
		public async Task<IActionResult> RequestPasswordReset(string email)
		{
			return await RequestPasswordReset_Internal(email);
		}

		[HttpPost(RESET_PASSWORD)]
		public async Task<IActionResult> ResetPassword(string token, [FromBody]string password)
		{
			return await ResetPassword_Internal(token, password);
		}

		[HttpPost(CHANGE_PASSWORD)]
		public async Task<IActionResult> ChangePassword([FromBody]ChangePasswordModel model)
		{
			return await ChangePassword(model);
		}

		[HttpGet(VERIFY_EMAIL)]
		public async Task<IActionResult> VerifyEmail(string token)
		{
			return await VerifyEmail_Internal(token);
		}

		[HttpPost]
		[Route(REGISTER)]
		public async Task<IActionResult> Register([FromBody] UserSchema schema, [FromQuery]string token = null)
		{
			return await Register(schema, token);
		}
	}
}
