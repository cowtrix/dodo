﻿using System;
using System.Diagnostics;
using Dodo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;

namespace RESTTests
{

	public abstract class RESTTestBase<T> : TestBase where T:DodoResource
	{
		public abstract string CreationURL { get; }
		public abstract object GetCreationSchema(bool unique = false);

		[TestMethod]
		public virtual void CanCreate()
		{
			var sw = new Stopwatch();
			sw.Start();
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			Assert.IsNotNull(createdObj.Value<string>("GUID"));
			CheckCreatedObject(createdObj);
			Context.WriteLine($"Test took {sw.Elapsed}");
		}

		[TestMethod]
		public virtual void CannotCreateDuplicate()
		{
			RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, GetCreationSchema()),
				e => e.Message.Contains("Conflict"));
		}

		[TestMethod]
		public virtual void CannotPatchDuplicate()
		{
			var firstObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var secondObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema(true));
			AssertX.Throws<Exception>(() =>
				RequestJSON(secondObj.Value<string>("ResourceURL"), Method.PATCH, new { Name = firstObj.Value<string>("Name") }),
				e => e.Message.Contains("Conflict - resource may already exist"));
			RequestJSON(secondObj.Value<string>("ResourceURL"), Method.GET);
		}

		protected virtual void CheckCreatedObject(JObject obj) { }

		[TestMethod]
		public virtual void CanGet()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceObj = RequestJSON(obj.Value<string>("ResourceURL"), Method.GET);
			Assert.IsNotNull(resourceObj.Value<string>("GUID"));
			CheckGetObject(resourceObj);
		}

		protected virtual void CheckGetObject(JObject obj) { }

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
			CheckPatchedObject(patch);
		}
		protected virtual void CheckPatchedObject(JObject obj) { }

		[TestMethod]
		public virtual void CannotPatchInvalid()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			AssertX.Throws<Exception>(() => RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, new { FakeField = "Not a field" }),
				x => x.Message.Contains("Invalid field names"));
		}
	}
}
