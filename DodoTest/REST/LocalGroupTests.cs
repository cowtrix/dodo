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
			RegisterUser(out var defaultGuid, DefaultUsername, DefaultName, DefaultPassword, DefaultEmail);
			DefaultGUID = defaultGuid;
		}

		public override object GetCreationSchema(bool unique)
		{
			if (!unique)
			{
				return new LocalGroupRESTHandler.CreationSchema("Test Local Group ", new GeoLocation(87, 14), "Test description"); ;
			}
			return new LocalGroupRESTHandler.CreationSchema ("Test Local Group " + StringExtensions.RandomString(6),
				new GeoLocation(87, 14), "Test description");
		}

		public override object GetPatchSchema()
		{
			return new { Location = new GeoLocation(14, 87) };
		}

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new LocalGroupRESTHandler.CreationSchema("Create", new GeoLocation(87, 14), "Test description")),
				e => e.Message.Contains("Reserved Resource URL"));
		}

		[TestMethod]
		public void CanList()
		{
			var objects = new List<JObject>()
			{
				RequestJSON(CreationURL, Method.POST, new LocalGroupRESTHandler.CreationSchema("Test1", new GeoLocation(87, 14), "Test description")),
				RequestJSON(CreationURL, Method.POST, new LocalGroupRESTHandler.CreationSchema("Test2", new GeoLocation(87, 14), "Test description")),
				RequestJSON(CreationURL, Method.POST, new LocalGroupRESTHandler.CreationSchema("Test3", new GeoLocation(87, 14), "Test description")),
				RequestJSON(CreationURL, Method.POST, new LocalGroupRESTHandler.CreationSchema("Test4", new GeoLocation(87, 14), "Test description")),
			};
			var guids = objects.Select(x => x.Value<string>("GUID"));
			var list = Request("localgroups", Method.GET);
			Assert.IsTrue(guids.All(guid => list.Content.Contains(guid)));
		}
	}
}
