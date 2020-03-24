using Dodo.SharedTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System;

namespace Dodo.UnitTests
{
	[TestClass]
	public class DateTimeTests : BasicObjectTests<DateTime>
	{
		protected override Func<DateTime, DateTime, bool> EqualityFunction => (first, second) => Math.Abs((first - second).TotalMinutes) < 1;

		protected override DateTime Get()
		{
			return SchemaGenerator.RandomDate;
		}
	}
}