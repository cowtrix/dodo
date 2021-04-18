using Common;
using Common.Extensions;
using Common.Security;
using Dodo.Users;
using Microsoft.AspNetCore.Mvc;
using Resources;
using Resources.Security;
using System;
using System.Threading.Tasks;

namespace Dodo.Email
{
	public class UnsubscribeModel
	{
		public string Email;
		public string Hash;
	}

	[Route(UNSUBSCRIBE)]
	public class UnsubscribeController : CustomController
	{
		public const string UNSUBSCRIBE = "unsubscribe";

		[HttpGet]
		public async Task<IActionResult> Index([FromQuery]string email, [FromQuery] string token)
		{
			if (string.IsNullOrEmpty(email))
			{
				return Redirect("~/");
			}
			var calcHash = EmailUtility.GetEmailHash(email);
			if (token != calcHash)
			{
				return BadRequest("Bad signature");
			}
			return View(new UnsubscribeModel { Email = email, Hash = token });
		}

		[HttpGet]
		[Route(nameof(Confirm))]
		[ValidateAntiForgeryToken]
		public async Task<IActionResult> Confirm([FromQuery] string hash)
		{
			EmailUtility.UnsubscribeHash(hash);
			return Redirect("~/");
		}
	}
}
