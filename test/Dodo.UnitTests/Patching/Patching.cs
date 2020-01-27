using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;
using System.Collections.Generic;
using System.Text;
using REST;
using Common.Security;
using Common.Extensions;
using System.Linq;
using SharedTest;

namespace Patching
{
	public abstract class Patching<T>
	{
		protected static Random Random = new Random();
		protected abstract IEnumerable<T> SampleValues { get; }

		[ViewClass]
		struct SingleFieldStructWithViewClass
		{
			public T Value;
		}

		struct SingleFieldStructWithViewAttr
		{
			[View(EPermissionLevel.PUBLIC)]
			public T Value;
		}

		class SingleFieldClassWithViewAttr
		{
			[View(EPermissionLevel.PUBLIC)]
			public T Value;
		}

		[TestMethod]
		public void PatchInClass()
		{
			var dataPairs = SampleValues.ChunkRandom(2);
			foreach (var pair in dataPairs)
			{
				var value1 = pair.First();
				var value2 = pair.Last();
				var patch = new Dictionary<string, object>()
				{
					{ "Value", value2}
				};
				var cl2 = new SingleFieldClassWithViewAttr { Value = value1 };
				cl2.PatchObject(patch);
				Assert.AreEqual(value2, cl2.Value);
			}
		}

		[TestMethod]
		public void PatchInStruct()
		{
			var dataPairs = SampleValues.ChunkRandom(2);
			foreach(var pair in dataPairs)
			{
				var value1 = pair.First();
				var value2 = pair.Last();
				var patch = new Dictionary<string, object>()
				{
					{ "Value", value2}
				};
				var str = new SingleFieldStructWithViewClass { Value = value1 };
				str = str.PatchObject(patch);
				Assert.AreEqual(value2, str.Value);
			}
		}

		[TestMethod]
		public void CannotPatchInStruct()
		{
			var dataPairs = SampleValues.ChunkRandom(2);
			foreach (var pair in dataPairs)
			{
				var value1 = pair.First();
				var value2 = pair.Last();
				var patch = new Dictionary<string, object>()
				{
					{ "Value", value2}
				};
				var str2 = new SingleFieldStructWithViewAttr { Value = value1 };
				AssertX.Throws<Exception>(() => str2.PatchObject(patch), e => e.Message.Contains("Cannot patch immutable struct - you must pass the full object."));
			}
		}
	}

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
