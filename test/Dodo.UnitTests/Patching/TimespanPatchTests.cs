using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Patching
{
	[TestClass]
	public class TimespanPatchTests : Patching<TimeSpan>
	{
		public static TimeSpan RandomTimespan => TimeSpan.FromSeconds(Random.Next(0, (int)TimeSpan.FromDays(365 * 50).TotalSeconds));
		
		protected override IEnumerable<TimeSpan> SampleValues => new List<TimeSpan>()
		{
			default,
			TimeSpan.MaxValue,
			TimeSpan.MinValue,
			RandomTimespan,
			RandomTimespan,
			RandomTimespan,
			RandomTimespan,
			RandomTimespan,
			RandomTimespan,
		};
	}
}
