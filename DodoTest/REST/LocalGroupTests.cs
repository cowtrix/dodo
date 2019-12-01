using Common;
using Common.Extensions;
using Dodo.LocalGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;

namespace RESTTests
{
	[TestClass]
	public class LocalGroupTests : GroupResourceTestBase<LocalGroup>
	{
		public override string CreationURL => LocalGroup.ROOT + "/create";

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(out var defaultGuid, DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			DefaultGUID = defaultGuid;
		}

		public override object GetCreationSchema(bool unique)
		{
			if (!unique)
			{
				return new LocalGroupRESTHandler.CreationSchema
				{
					Name = "Test Local Group ",
					Location = new GeoLocation(87, 14)
				};
			}
			return new LocalGroupRESTHandler.CreationSchema { Name = "Test Local Group " + StringExtensions.RandomString(6),
				Location = new GeoLocation(87, 14) };
		}

		public override object GetPatchSchema()
		{
			return new { Location = new GeoLocation(14, 87) };
		}

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new LocalGroupRESTHandler.CreationSchema { Name = "Create" }),
				e => e.Message.Contains("Reserved Resource URL"));
		}

		[TestMethod]
		public void CanList()
		{
			var objects = new List<JObject>()
			{
				RequestJSON(CreationURL, Method.POST, new LocalGroupRESTHandler.CreationSchema { Name = "Test1" }),
				RequestJSON(CreationURL, Method.POST, new LocalGroupRESTHandler.CreationSchema { Name = "Test2" }),
				RequestJSON(CreationURL, Method.POST, new LocalGroupRESTHandler.CreationSchema { Name = "Test3" }),
				RequestJSON(CreationURL, Method.POST, new LocalGroupRESTHandler.CreationSchema { Name = "Test4" }),
			};
			var guids = objects.Select(x => x.Value<string>("GUID"));
			var list = Request("localgroups", Method.GET);
			Assert.IsTrue(guids.All(guid => list.Content.Contains(guid)));
		}
	}
}
