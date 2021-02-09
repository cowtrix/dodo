using Microsoft.AspNetCore.Mvc;
using Resources;
using System.Threading.Tasks;

namespace Dodo.Sites
{
	[Route(RootURL)]
	public class StaticResourceController : CustomController
	{
		public const string RootURL = "rsc";
		public const string PrivacyPolicyURL = "privacypolicy";
		public const string RebelAgreementURL = "rebelagreement";

		[HttpGet(PrivacyPolicyURL)]
		public IActionResult PrivacyPolicy()
		{
			return View();
		}

		[HttpGet(RebelAgreementURL)]
		public IActionResult RebelAgreement()
		{
			return View();
		}
	}
}
