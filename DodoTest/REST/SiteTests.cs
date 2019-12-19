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
				new RebellionRESTHandler.CreationSchema("Test Rebellion", "Test description", new GeoLocation(45, 97)));
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
			return new { Name = "New site name", Facilities = new { Toilets = "Free", TalksAndTraining = true } };
		}

		protected override void CheckPatchedObject(JObject obj)
		{
			Assert.AreEqual("New site name", obj.Value<string>("Name"));
			Assert.AreEqual("Free", obj.Value<JObject>("Facilities").Value<string>("Toilets"));
			Assert.AreEqual(true, obj.Value<JObject>("Facilities").Value<bool>("TalksAndTraining"));
			m_postman.UpdateExampleJSON(obj.ToString(), "Sites", "Update a Site");
		}

		protected override void CheckCreatedObject(JObject obj)
		{
			Assert.AreEqual(Rebellion.Value<string>("GUID"), obj.Value<JObject>("Rebellion").Value<string>("Guid"));
		}

		protected override void CheckGetObject(JObject obj)
		{
			base.CheckGetObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Sites", "Get a Site");
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
			var obj = RequestJSON(CreationURL, Method.POST,
				new SiteRESTHandler.CreationSchema("Test1", typeof(ActionSite).FullName, new GeoLocation(27, 79), "Test description"));
			CheckCreatedObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Sites", "Create a new Action Site");
		}

		[TestMethod]
		public void CanCreateEventSite()
		{
			var obj = RequestJSON(CreationURL, Method.POST,
				new SiteRESTHandler.CreationSchema("Test1", typeof(Event).FullName, new GeoLocation(27, 79), "Test description"));
			CheckCreatedObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Sites", "Create a new Event Site");
		}

		[TestMethod]
		public void CanCreateSanctuarySite()
		{
			var obj = RequestJSON(CreationURL, Method.POST,
				new SiteRESTHandler.CreationSchema("Test1", typeof(Sanctuary).FullName, new GeoLocation(27, 79), "Test description"));
			CheckCreatedObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Sites", "Create a new Sanctuary Site");
		}

		[TestMethod]
		public void CanCreateMarchSite()
		{
			var obj = RequestJSON(CreationURL, Method.POST,
				new SiteRESTHandler.CreationSchema("Test1", typeof(March).FullName, new GeoLocation(27, 79), "Test description"));
			CheckCreatedObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Sites", "Create a new March");
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
			m_postman.UpdateExampleJSON(list.Content, "Sites", "List all Sites");
		}
	}
}
