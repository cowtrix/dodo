using System.IO;
using Common.Extensions;

namespace Dodo.Static
{
	public class AboutPage : StaticMarkdownContent
	{
		public readonly string Title;
		public readonly string Text;

		public AboutPage()
		{
			Title = $"About {DodoApp.PRODUCT_NAME}";
			Text = File.ReadAllText(Path.Combine("Content", "About.md"))
				.ReplaceAll(VariableTemplates);
		}
	}
}
