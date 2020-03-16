using Dodo.SharedTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using MongoDB.Bson.Serialization.Attributes;
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
		}

		protected override TestClass Get()
		{
			return new TestClass();
		}
	}
}
