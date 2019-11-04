﻿using System;
using System.Collections.Generic;
using Dodo.Rebellions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace DodoTest
{
	[TestClass]
	public class SimpleTests : TestBase
	{
		[TestMethod]
		public void CanRegisterNewUser()
		{
			var newUser = RegisterUser(CurrentLogin, CurrentPassword);
			Assert.IsTrue(newUser.Value<JObject>("WebAuth").Value<string>("Username") == CurrentLogin);
		}

		[TestMethod]
		public void CanCreateNewRebellion()
		{
			var newUser = RegisterUser(CurrentLogin, CurrentPassword);
			var rebellion = CreateNewRebellion("Test Rebellion", new GeoLocation(66, 66));
			Assert.IsTrue(rebellion.Value<string>("RebellionName") == "Test Rebellion");
			Assert.IsTrue(rebellion.Value<JObject>("Location").Value<double>("Latitude") == 66);
			Assert.IsTrue(rebellion.Value<JObject>("Location").Value<double>("Longitude") == 66);
		}
	}
}
