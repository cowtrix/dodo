using System;
using System.Collections.Generic;
using Common;
using Dodo;
using Dodo.Rebellions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RESTTests
{

	public abstract class RESTTestBase<T> : TestBase where T:DodoResource
	{
		public abstract string CreationURL { get; }
		public abstract object GetCreationSchema();

		[TestMethod]
		public virtual void CanCreate()
		{
			RequestJSON(CreationURL, Method.POST, GetCreationSchema());
		}

		[TestMethod]
		public virtual void CanGet()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			RequestJSON(obj.Value<string>("ResourceURL"), Method.GET);
		}

		[TestMethod]
		public virtual void CanDestroy()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var response = Request(obj.Value<string>("ResourceURL"), Method.DELETE);
			Assert.IsTrue(response.StatusDescription.Contains("Resource deleted"));
		}

		[TestMethod]
		public virtual void CanGetByResource()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			RequestJSON("resources/" + obj.Value<string>("GUID"), Method.GET);
		}

		public abstract object GetPatchSchema();
		[TestMethod]
		public virtual void CanPatch()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var patch = RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, GetPatchSchema());
			Assert.AreNotEqual(obj.ToString(), patch.ToString());
		}

		/*

		[TestMethod]
		public void CanGetUserByResource()
		{
			var newUser = RegisterUser(CurrentLogin, CurrentPassword);
			var guid = newUser.Value<string>("UUID");
			var newUserResource = GetResource(guid);
			Assert.IsTrue(newUser.ToString() == newUserResource.ToString());
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
		public void CanCreateNewRebellion()
		{
			var newUser = RegisterUser(CurrentLogin, CurrentPassword);
			var rebellion = CreateNewRebellion("Test Rebellion", new GeoLocation(66, 66));
			Assert.IsTrue(rebellion.Value<string>("RebellionName") == "Test Rebellion");
			Assert.IsTrue(rebellion.Value<JObject>("Location").Value<double>("Latitude") == 66);
			Assert.IsTrue(rebellion.Value<JObject>("Location").Value<double>("Longitude") == 66);
		}

		*/
	}
}
