using Common;
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
		public override string CreationURL => $"{Rebellion}/{WorkingGroup.ROOT}/create";

		private string Rebellion { get; set; }

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			Rebellion = RequestJSON("rebellions/create", Method.POST, new RebellionRESTHandler.CreationSchema { Name = "Test Rebellion", Location = new GeoLocation(45, 97) })
				.Value<string>("ResourceURL");
		}

		public override object GetCreationSchema()
		{
			return new WorkingGroupRESTHandler.CreationSchema("Test Working Group");
		}

		public override object GetPatchSchema()
		{
			return new { Mandate = "This is a test mandate" };
		}

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new WorkingGroupRESTHandler.CreationSchema("Create")),
				e => e.Message.Contains("Reserved Resource URL"));
		}

		[TestMethod]
		public async Task SendEmail()
		{
			await EmailHelper.Execute();
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
			var list = Request($"{Rebellion}/{WorkingGroup.ROOT}/", Method.GET);
			Assert.IsTrue(guids.All(guid => list.Content.Contains(guid)));
		}
	}
}
