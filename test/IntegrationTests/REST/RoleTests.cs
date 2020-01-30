﻿using Common;
using Common.Extensions;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using SharedTest;
using System;
using System.Linq;

namespace RESTTests
{
	[TestClass]
	public class RoleTests : RESTTestBase<Role>
	{
		public override string ResourceRoot => RoleController.RootURL;

		/*[TestInitialize]
		public void Setup()
		{
			RegisterUser(out _, DefaultUsername, "Test User", DefaultPassword, "test@web.com");
			var rebellion = RequestJSON("rebellions/create", Method.POST,
				new RebellionSchema("Test Rebellion", "Test description", new GeoLocation(45, 97), RebellionTests.DefaultStart, RebellionTests.DefaultEnd));
			WorkingGroup = RequestJSON(rebellion.Value<string>("ResourceURL") + "/wg/create", Method.POST,
				new WorkingGroupSchema("Test Working Group", "Test mandate", rebellion.Value<Guid>("GUID")));
		}

		public override object GetCreationSchema(bool unique)
		{
			if(!unique)
			{
				return new RoleSchema("Test Role ", "Test mandate", WorkingGroup.Value<Guid>("GUID"));
			}
			return new RoleSchema("Test Role " + StringExtensions.RandomString(6), "Test mandate", WorkingGroup.Value<Guid>("GUID"));
		}

		public override object GetPatchSchema()
		{
			return new { PublicDescription = "New description" };
		}

		protected override void CheckPatchedObject(JObject obj)
		{
			Assert.AreEqual("New description", obj.Value<string>("PublicDescription"));
			m_postman.UpdateExampleJSON(obj.ToString(), "Roles", "Update a Role");
		}

		protected override void CheckCreatedObject(JObject obj)
		{
			var workingGroupGUID = WorkingGroup.Value<string>("GUID");
			var workingGroupGUIDFromObj = obj.Value<JObject>("Parent").Value<string>("Guid");
			Assert.AreEqual(workingGroupGUID, workingGroupGUIDFromObj);
			var parent = RequestJSON(WorkingGroup.Value<string>("ResourceURL"), Method.GET);
			Assert.IsTrue(parent.Value<JArray>("Roles").Values<string>().Contains(obj.Value<string>("GUID")));
			m_postman.UpdateExampleJSON(obj.ToString(), "Roles", "Create a new Role");
		}

		protected override void CheckGetObject(JObject obj)
		{
			base.CheckGetObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Roles", "Get a Role");
		}*/
	}
}
