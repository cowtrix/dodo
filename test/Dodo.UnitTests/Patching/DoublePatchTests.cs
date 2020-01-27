using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;

namespace Patching
{
	[TestClass]
	public class DoublePatchTests : Patching<double>
	{
		protected override IEnumerable<double> SampleValues => new List<double>()
		{
			default,
			Math.PI,
			-234.49583496,
			4562342342.4086,
			double.MaxValue,
			double.MinValue,
			double.NaN,
			double.NegativeInfinity,
			double.PositiveInfinity,
			234234.34656,
			99.54,
			1.2,
			Random.NextDouble(),
			Random.NextDouble(),
			Random.NextDouble(),
			Random.NextDouble(),
			(Random.NextDouble() - 0.5) * 2 * double.MaxValue,
			(Random.NextDouble() - 0.5) * 2 * double.MaxValue,
			(Random.NextDouble() - 0.5) * 2 * double.MaxValue,
			(Random.NextDouble() - 0.5) * 2 * double.MaxValue,
			(Random.NextDouble() - 0.5) * 2 * double.MaxValue,
			(Random.NextDouble() - 0.5) * 2 * double.MaxValue,
		};
	}
}
