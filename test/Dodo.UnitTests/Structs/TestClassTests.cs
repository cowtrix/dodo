using Dodo.SharedTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson.Serialization.Attributes;
using Resources;
using System;

namespace Dodo.UnitTests
{
	[TestClass]
	public class TestClassTests : BasicObjectTests<TestClassTests.TestClass>
	{
		public class TestClass
		{
			[BsonDateTimeOptions(Kind = DateTimeKind.Utc)]
			public DateTime DT = SchemaGenerator.RandomDate;
			public GeoLocation Location = SchemaGenerator.RandomLocation;
		}

		protected override TestClass Get()
		{
			return new TestClass();
		}

		protected override Func<TestClass, TestClass, bool> EqualityFunction => (a, b) =>
		{
			return a.DT == b.DT
				&& a.Location == b.Location;
		};
	}
}
