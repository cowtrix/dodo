using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Collections.Generic;
using Common.Security;
using Common.Extensions;

namespace Patching
{
	[TestClass]
	public class StringPatchtests : Patching<string>
	{
		protected override IEnumerable<string> SampleValues => new List<string>()
		{
			"",
			"",
			"abc",
			"defg",
			"asojdkjfsdljnjsdkjgfj",
			"!\"£$%^&*()-}{#~@'?/><.,\\|`¬",
			"日本語,",
			KeyGenerator.GetUniqueKey(64),
			KeyGenerator.GetUniqueKey(128),
			StringExtensions.RandomString(128),
			StringExtensions.RandomString(457),
			StringExtensions.RandomString(2340),
		};
	}
}
