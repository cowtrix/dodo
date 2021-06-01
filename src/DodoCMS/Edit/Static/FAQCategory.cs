using Common.Extensions;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text.RegularExpressions;

namespace Dodo.Static
{
	public abstract class StaticMarkdownContent
	{
		protected static Dictionary<string, string> VariableTemplates = new Dictionary<string, string>
		{
			{ "{URL}", Dodo.DodoApp.NetConfig.FullURI },
			{ "{PRODUCT}", Dodo.DodoApp.PRODUCT_NAME },
			{"{SUPPORT_EMAIL}", Dodo.DodoApp.SupportEmail },
		};
	}

	public class FAQCategory : StaticMarkdownContent
	{
		protected static string RemoveOrderString(string str)
		{
			return Regex.Replace(str, @"^\d+\s*-\s*", "");
		}

		public class Entry
		{
			public Entry(string category, string path)
			{
				EntryPath = path;
				Question = $"{RemoveOrderString(Path.GetFileNameWithoutExtension(EntryPath))}?";
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
			CategoryName = RemoveOrderString(Path.GetFileNameWithoutExtension(CategoryPath));
			Slug = ValidationExtensions.StripStringForSlug(CategoryName); ;
			Entries = new List<Entry>(Directory.GetFiles(CategoryPath, "*.md").Select(f => new Entry(CategoryName, f)));
		}

		public string CategoryPath { get; }
		public string CategoryName { get; }
		public string Slug { get; }
		public IReadOnlyList<Entry> Entries { get; }

	}
}
