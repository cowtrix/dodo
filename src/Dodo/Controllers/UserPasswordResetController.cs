using Resources;
using Microsoft.AspNetCore.Mvc;
using System.Threading.Tasks;
using static UserService;
using Common.Extensions;

#pragma warning disable CS1998 // Async method lacks 'await' operators and will run synchronously

namespace Dodo.Users
{
	public class PasswordResetModel : IVerifiable
	{
		public string Token { get; set; }
		[Password]
		public string NewPassword { get; set; }
		[Password]
		public string NewPasswordConfirm { get; set; }

		public bool CanVerify() => true;

		public bool VerifyExplicit(out string error)
		{
			if (NewPassword != NewPasswordConfirm)
			{
				error = "Passwords did not match.";
				return false;
			}
			error = null;
			return true;
		}
	}

	[Route(REDEEM_PASSWORD_TOKEN)]
	public class UserPasswordResetController : CustomController
	{
		protected UserService UserService => new UserService(Context, HttpContext, new UserAuthService());

		[HttpGet]
		public async Task<IActionResult> RedeemPasswordToken([FromQuery] string token)
		{
			var model = new PasswordResetModel { Token = token };
			return View(model);
		}

		[HttpPost(SUBMIT_PASSWORD_RESET)]
		public async Task<IActionResult> SubmitPasswordReset([FromForm] PasswordResetModel model)
		{
			if (!ModelState.IsValid)
			{
				return View(nameof(RedeemPasswordToken), model);
			}
			if (!model.Verify(out var error))
			{
				ModelState.AddModelError("", "Passwords must match");
				return View(nameof(RedeemPasswordToken), model);
			}
			var result = await UserService.ResetPassword(model.Token, model.NewPassword);
			if(result.IsSuccess)
			{
				return result.ActionResult;
			}
			var msg = (ResourceRequestError)result;
			ModelState.AddModelError("", msg.Message);
			return View(nameof(RedeemPasswordToken), model);
		}
	}
}
