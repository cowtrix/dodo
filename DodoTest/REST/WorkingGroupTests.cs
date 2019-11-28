﻿using Common;
using Dodo.Rebellions;
using Dodo.Users;
using Dodo.Utility;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace RESTTests
{
	[TestClass]
	public class WorkingGroupTests : GroupResourceTestBase<WorkingGroup>
	{
		public override string CreationURL => $"{Rebellion.Value<string>("ResourceURL")}/{WorkingGroup.ROOT}/create";

		private JObject Rebellion { get; set; }

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			Rebellion = RequestJSON("rebellions/create", Method.POST, 
				new RebellionRESTHandler.CreationSchema { Name = "Test Rebellion", Location = new GeoLocation(45, 97) });
		}

		public override object GetCreationSchema()
		{
			return new WorkingGroupRESTHandler.CreationSchema("Test Working Group");
		}

		public override object GetPatchSchema()
		{
			return new { Mandate = "This is a test mandate" };
		}

		protected override void CheckCreatedObject(JObject obj)
		{
			Assert.AreEqual(Rebellion.Value<string>("GUID"), obj.Value<JObject>("Parent").Value<string>("Guid"));
		}

		[TestMethod]
		public void CanCreateSubWorkingGroup()
		{
			var wg = RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test Working Group"));
			var subwg = RequestJSON(wg.Value<string>("ResourceURL") + "/wg/create",
				Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test Working Group"));
			Assert.IsTrue(subwg.Value<JArray>("SubGroups").Values<string>().All(x => x == wg.Value<string>("GUID")));
		}

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Create")),
				e => e.Message.Contains("Reserved Resource URL"));
		}

		[TestMethod]
		public void CanList()
		{
			var objects = new List<JObject>()
			{
				RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test1")),
				RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test2")),
				RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test3")),
				RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Test4")),
			};
			var guids = objects.Select(x => x.Value<string>("GUID"));
			var list = Request($"{Rebellion.Value<string>("ResourceURL")}/{WorkingGroup.ROOT}/", Method.GET);
			Assert.IsTrue(guids.All(guid => list.Content.Contains(guid)));
		}
	}
}
