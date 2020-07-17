using Common.Extensions;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace UnitTests
{
	[TestClass]
	public class StringTests
	{
		[DataTestMethod]
		[DataRow("[test](www.test.com)", "<a href=\"www.test.com\">test</a>")]
		[DataRow("This is a test [my cool test](https://www.test.com) la la.\nEven After a newline, [this works](myurl.com)!", 
			"This is a test <a href=\"https://www.test.com\">my cool test</a> la la.</br>Even After a newline, <a href=\"myurl.com\">this works</a>!")]
		[DataRow("naughty naughty! <iframe src=\"https://www.w3schools.com\" title=\"W3Schools Free Online Web Tutorials\"></iframe>",
			"naughty naughty! &lt;iframe src=\"https://www.w3schools.com\" title=\"W3Schools Free Online Web Tutorials\"&gt;&lt;/iframe&gt;")]
		public void TextToHTML(string md, string html)
		{
			Assert.AreEqual(html, StringExtensions.TextToHtml(md));
		}
	}

}
