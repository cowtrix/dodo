using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Patching
{
	[TestClass]
	public class DateTimePatchTests : Patching<DateTime>
	{
		private DateTime RandomDateTime => new DateTime(2000, 1, 1) + TimespanPatchTests.RandomTimespan;

		protected override IEnumerable<DateTime> SampleValues => new List<DateTime>()
		{
			default,
			DateTime.Now,
			DateTime.MinValue,
			DateTime.MaxValue,
			RandomDateTime,
			RandomDateTime,
			RandomDateTime,
			RandomDateTime,
			RandomDateTime,
			RandomDateTime,
		};
	}
}
