using Common;
using Common.Extensions;
using Dodo.Rebellions;
using Dodo.Sites;
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
	public class SiteTests : RESTTestBase<Site>
	{
		public override string CreationURL => $"{Rebellion.Value<string>("ResourceURL")}/{Site.ROOT}/create";

		private JObject Rebellion { get; set; }

		[TestInitialize]
		public void Setup()
		{
			RegisterUser(out _, DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			Rebellion = RequestJSON("rebellions/create", Method.POST,
				new RebellionRESTHandler.CreationSchema { Name = "Test Rebellion", Location = new GeoLocation(45, 97) });
		}

		public override object GetCreationSchema(bool unique)
		{
			if(unique)
			{
				return new SiteRESTHandler.CreationSchema("Test Site " + StringExtensions.RandomString(6),
					typeof(OccupationalSite).FullName, new GeoLocation(27, 79), "Test description");
			}
			return new SiteRESTHandler.CreationSchema("Test Site", typeof(OccupationalSite).FullName, new GeoLocation(27, 79), "Test description");
		}

		public override object GetPatchSchema()
		{
			return new { Name = "New site name" };
		}

		protected override void CheckPatchedObject(JObject obj)
		{
			Assert.AreEqual("New site name", obj.Value<string>("Name"));
		}

		protected override void CheckCreatedObject(JObject obj)
		{
			Assert.AreEqual(Rebellion.Value<string>("GUID"), obj.Value<JObject>("Rebellion").Value<string>("Guid"));
		}

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST,
				new SiteRESTHandler.CreationSchema("Create", typeof(OccupationalSite).FullName, new GeoLocation(27, 79), "Test description")),
				e => e.Message.Contains("Reserved Resource URL"));
		}

		[TestMethod]
		public void CanCreateActionSite()
		{
			CheckCreatedObject(RequestJSON(CreationURL, Method.POST,
				new SiteRESTHandler.CreationSchema("Test1", typeof(ActionSite).FullName, new GeoLocation(27, 79), "Test description")));
		}

		[TestMethod]
		public void CanCreateSanctuarySite()
		{
			CheckCreatedObject(RequestJSON(CreationURL, Method.POST,
				new SiteRESTHandler.CreationSchema("Test1", typeof(Sanctuary).FullName, new GeoLocation(27, 79), "Test description")));
		}

		[TestMethod]
		public void CanCreateMarchSite()
		{
			CheckCreatedObject(RequestJSON(CreationURL, Method.POST,
				new SiteRESTHandler.CreationSchema("Test1", typeof(March).FullName, new GeoLocation(27, 79), "Test description")));
		}

		[TestMethod]
		public void CanList()
		{
			var objects = new List<JObject>()
			{
				RequestJSON(CreationURL, Method.POST, new SiteRESTHandler.CreationSchema("Test1", typeof(OccupationalSite).FullName, new GeoLocation(27, 79.2), "Test description")),
				RequestJSON(CreationURL, Method.POST, new SiteRESTHandler.CreationSchema("Test2", typeof(ActionSite).FullName, new GeoLocation(26.9, 79), "Test description")),
				RequestJSON(CreationURL, Method.POST, new SiteRESTHandler.CreationSchema("Test3", typeof(March).FullName, new GeoLocation(27.2, 79.1), "Test description")),
				RequestJSON(CreationURL, Method.POST, new SiteRESTHandler.CreationSchema("Test4", typeof(Sanctuary).FullName, new GeoLocation(26.4, 78.7), "Test description")),
			};
			var guids = objects.Select(x => x.Value<string>("GUID"));
			var list = Request($"{Rebellion.Value<string>("ResourceURL")}/{Site.ROOT}/", Method.GET);
			Assert.IsTrue(guids.All(guid => list.Content.Contains(guid)));
		}
	}
}
