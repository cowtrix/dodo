using System;
using System.Collections.Generic;
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
			var newUser = RegisterUser("Test", "password");
			Assert.IsTrue(newUser.Value<JObject>("WebAuth").Value<string>("Username") == "Test");
		}
	}
}
