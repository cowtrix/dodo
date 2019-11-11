﻿using System;
using Dodo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
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
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			Assert.IsNotNull(createdObj.Value<string>("GUID"));
		}

		[TestMethod]
		public virtual void CanGet()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceObj = RequestJSON(obj.Value<string>("ResourceURL"), Method.GET);
			Assert.IsNotNull(resourceObj.Value<string>("GUID"));
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
			var resourceObj = RequestJSON("resources/" + obj.Value<string>("GUID"), Method.GET);
			Assert.AreEqual(resourceObj.Value<string>("GUID"), obj.Value<string>("GUID"));
		}

		public abstract object GetPatchSchema();
		[TestMethod]
		public virtual void CanPatch()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var patch = RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, GetPatchSchema());
			Assert.AreNotEqual(obj.ToString(), patch.ToString());
		}

		[TestMethod]
		public virtual void CannotPatchInvalid()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			AssertX.Throws<Exception>(() => RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, new { FakeField = "Not a field" }),
				x => x.Message.Contains("Invalid field names"));
		}
	}
}
