using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Patching
{
	[TestClass]
	public class IntPatchTests : Patching<int>
	{
		protected override IEnumerable<int> SampleValues => new List<int>()
		{
			default,
			123456,
			int.MaxValue,
			int.MinValue,
			345734,
			-38445,
			512,
			256,
			Random.Next(int.MinValue, int.MaxValue),
			Random.Next(int.MinValue, int.MaxValue),
			Random.Next(int.MinValue, int.MaxValue),
			Random.Next(int.MinValue, int.MaxValue),
			Random.Next(int.MinValue, int.MaxValue),
			Random.Next(int.MinValue, int.MaxValue),
			Random.Next(int.MinValue, int.MaxValue),
		};
	}
}
