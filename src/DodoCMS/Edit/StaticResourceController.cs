using Common.Extensions;
using Microsoft.AspNetCore.Mvc;
using Resources;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Threading.Tasks;

namespace Dodo.Static
{
	public class FAQCategory
	{
		private static Dictionary<string, string> VariableTemplates = new Dictionary<string, string>
		{
			{ "{URL}", Dodo.DodoApp.NetConfig.FullURI }
		};

		public class Entry
		{
			public Entry(string category, string path)
			{
				EntryPath = path;
				Question = $"{Path.GetFileNameWithoutExtension(EntryPath)}?";
				Content = Markdig.Markdown.ToHtml(File.ReadAllText(EntryPath).ReplaceAll(VariableTemplates));
				Slug = ValidationExtensions.StripStringForSlug($"{category}_{Question}");
			}
			public string EntryPath { get; }
			public string Question { get; }
			public string Content { get; }
			public string Slug { get; }
		}

		public FAQCategory(string catPath)
		{
			CategoryPath = catPath;
			CategoryName = Path.GetFileNameWithoutExtension(CategoryPath);
			Slug = ValidationExtensions.StripStringForSlug(CategoryName); ;
			Entries = new List<Entry>(Directory.GetFiles(CategoryPath, "*.md").Select(f => new Entry(CategoryName, f)));
		}

		public string CategoryPath { get; }
		public string CategoryName { get; }
		public string Slug { get; }
		public IReadOnlyList<Entry> Entries { get; }

	}

	[Route(RootURL)]
	public class StaticResourceController : CustomController
	{
		public const string RootURL = "rsc";
		public const string PrivacyPolicyURL = "privacypolicy";
		public const string RebelAgreementURL = "rebelagreement";
		public const string AboutURL = "about";
		public const string FAQURL = "faq";

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
			return View();
		}

		[HttpGet(FAQURL)]
		public IActionResult FAQ()
		{
			var categories =
				Directory.GetDirectories(System.IO.Path.Combine("Content", "FAQ"))
				.Select(catPath => new FAQCategory(catPath));
			return View(categories);
		}
	}
}
