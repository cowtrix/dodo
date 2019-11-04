using System;
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
		public void CanPatchUser()
		{
			RegisterUser(CurrentLogin, CurrentPassword);
			var patchObj = new { Email = "Patched Value" };
			var patchedUser = PatchObject("u/" + CurrentLogin, patchObj);
			Assert.IsTrue(patchedUser.Value<string>("Email") == patchObj.Email);
		}

		[TestMethod]
		public void CanGetUserByResource()
		{
			var newUser = RegisterUser(CurrentLogin, CurrentPassword);
			var guid = newUser.Value<string>("UUID");
			var newUserResource = GetResource(guid);
			Assert.IsTrue(newUser.ToString() == newUserResource.ToString());
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

		[TestMethod]
		public void CannotCreateNewRebellionIfNotLoggedIn()
		{
			AssertX.Throws<Exception>(() => CreateNewRebellion("Test Rebellion", new GeoLocation(66, 66)), e => e.Message.Contains("You need to login"));
		}
	}
}
