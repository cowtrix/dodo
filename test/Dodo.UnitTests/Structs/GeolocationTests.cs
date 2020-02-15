using Common;
using Dodo.SharedTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using System;
using System.Collections.Generic;
using System.Text;

namespace Dodo.UnitTests
{

	[TestClass]
	public class GeolocationTests : CustomStructTests<GeoLocation>
	{
		protected override GeoLocation Get()
		{
			return SchemaGenerator.RandomLocation;
		}

		[TestMethod]
		public void CheckValues()
		{
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GeoLocation(256, 512));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GeoLocation(-256, -512));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GeoLocation(double.NaN, 512));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GeoLocation(256, double.NaN));
			Assert.ThrowsException<ArgumentOutOfRangeException>(() => new GeoLocation(double.PositiveInfinity, double.NegativeInfinity));
		}
	}
}
