using Microsoft.AspNetCore.Mvc;
using Resources;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dodo.Static
{

	[Route(RootURL)]
	public class StaticResourceController : CustomController
	{
		public const string RootURL = "rsc";
		public const string PrivacyPolicyURL = "privacypolicy";
		public const string RebelAgreementURL = "rebelagreement";
		public const string AboutURL = "about";
		public const string FAQURL = "faq";

		private IEnumerable<FAQCategory> FAQs =
			Directory.GetDirectories(System.IO.Path.Combine("Content", "FAQ"))
			.Select(catPath => new FAQCategory(catPath))
			.ToList();

		private AboutPage AboutInfo = new AboutPage();

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

		[HttpGet(AboutURL)]
		public IActionResult About()
		{
			return View(AboutInfo);
		}

		[HttpGet(FAQURL)]
		public IActionResult FAQ()
		{
			return View(FAQs);
		}
	}
}
