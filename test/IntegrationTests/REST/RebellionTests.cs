using Dodo.Rebellions;
using Dodo.SharedTest;
using DodoResources.Rebellions;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Resources;
using System;
using System.Threading.Tasks;
using Common.Extensions;
using System.Collections.Generic;
using System.Linq;
using Newtonsoft.Json.Linq;
using Common;

namespace RESTTests
{


	[TestClass]
	public class RebellionTests : GroupResourceTestBase<Rebellion, RebellionSchema>
	{
		public static DateTime DefaultStart => new DateTime(2019, 10, 7, 0, 0, 0, DateTimeKind.Utc);
		public static DateTime DefaultEnd => new DateTime(2019, 10, 14, 0, 0, 0, DateTimeKind.Utc);
		public override string ResourceRoot => RebellionController.RootURL;

		protected override void VerifyCreatedObject(Rebellion rebellion, JObject obj, RebellionSchema schema)
		{
			base.VerifyCreatedObject(rebellion, obj, schema);
			Assert.AreEqual(schema.Location, rebellion.Location);
			Assert.IsTrue(schema.StartDate - rebellion.StartDate < TimeSpan.FromMinutes(2), $"Start date wasn't close enough - delta was {schema.StartDate - rebellion.StartDate}");
			Assert.IsTrue(schema.EndDate - rebellion.EndDate < TimeSpan.FromMinutes(2), $"End date wasn't close enough - delta was {schema.EndDate - rebellion.EndDate}");
		}

		protected override JObject GetPatchObject()
		{
			var ret = new JObject();
			ret["Location"] = JToken.FromObject(SchemaGenerator.RandomLocation);
			ret["StartDate"] = SchemaGenerator.RandomDate;
			ret["EndDate"] = SchemaGenerator.RandomDate;
			ret["PublicDescription"] = "test test test";
			return ret;
		}

		protected override void VerifyPatchedObject(Rebellion rsc, JObject patchObj)
		{
			Assert.AreEqual(patchObj["Location"].ToObject<GeoLocation>(), rsc.Location);
			Assert.IsTrue(patchObj.Value<DateTime>("StartDate") - rsc.StartDate < TimeSpan.FromMinutes(2));
			Assert.IsTrue(patchObj.Value<DateTime>("EndDate") - rsc.EndDate < TimeSpan.FromMinutes(2));
			Assert.AreEqual(patchObj.Value<string>("PublicDescription"), rsc.PublicDescription);
		}

		/*public override object GetCreationSchema(bool unique = false)
		{
			if(!unique)
			{
				return new RebellionSchema("Test Rebellion", SchemaGenerator.SampleMarkdown, new GeoLocation(45, 97), DefaultStart, DefaultEnd);
			}
			return new RebellionSchema("Test Rebellion " + StringExtensions.RandomString(6), SchemaGenerator.SampleMarkdown, new GeoLocation(45, 97), DefaultStart, DefaultEnd);
		}

		public override object GetPatchSchema()
		{
			return new { Location = new GeoLocation(62, 41), StartDate = DefaultStart,
				EndDate = DefaultEnd };
		}

		protected override void CheckPatchedObject(JObject obj)
		{
			Assert.AreEqual(new GeoLocation(62, 41), obj.Value<JObject>("Location").ToObject<GeoLocation>());
			Assert.AreEqual(new DateTime(2019, 10, 7), obj.Value<DateTime>("StartDate"));
			Assert.AreEqual(new DateTime(2019, 10, 14), obj.Value<DateTime>("EndDate"));
			base.CheckPatchedObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Rebellions", "Create a new Rebellion");
		}

		protected override void CheckGetObject(JObject obj)
		{
			base.CheckGetObject(obj);
			m_postman.UpdateExampleJSON(obj.ToString(), "Rebellions", "Get a Rebellion");
		}

		[TestInitialize]
		public void Setup()
		{
			var newUser = ResourceUtility.GetFactory<User>().CreateObject(new UserSchema(DefaultName, DefaultUsername, DefaultPassword, DefaultEmail));
			DefaultGUID = newUser.GUID.ToString();
		}

		[TestMethod]
		public void CannotCreateNewRebellionIfNotLoggedIn()
		{
			AssertX.Throws<Exception>(() =>
			{
				RequestJSON(CreationURL, Method.POST, GetCreationSchema(), "", "");
			}, e => e.Message.Contains("Forbidden"));
		}

		[TestMethod]
		public void CannotCreateAtCreationURL()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new RebellionSchema("Create", "Test description", new GeoLocation(), DefaultStart, DefaultEnd)),
				e => e.Message.Contains("Reserved Resource URL"));
		}

		[TestMethod]
		public void CannotCreateWithBigName()
		{
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, new RebellionSchema(
				StringExtensions.RandomString(512), "Test description", new GeoLocation(), DefaultStart, DefaultEnd)),
				e => e.Message.Contains("Name length must be between 3 and 64 characters long"));
		}

		[TestMethod]
		public void CanList()
		{
			var objects = new List<JObject>()
			{
				RequestJSON(CreationURL, Method.POST, new RebellionSchema("Test1", "Test description", new GeoLocation(27, 79.2), DefaultStart, DefaultEnd)),
				RequestJSON(CreationURL, Method.POST, new RebellionSchema("Test2", "Test description", new GeoLocation(26.9, 79), DefaultStart, DefaultEnd)),
				RequestJSON(CreationURL, Method.POST, new RebellionSchema("Test3", "Test description", new GeoLocation(27.2, 79.1), DefaultStart, DefaultEnd)),
				RequestJSON(CreationURL, Method.POST, new RebellionSchema("Test4", "Test description", new GeoLocation(26.4, 78.7), DefaultStart, DefaultEnd)),
			};
			var guids = objects.Select(x => x.Value<string>("GUID"));
			var list = Request("rebellions", Method.GET);
			Assert.IsTrue(guids.All(guid => list.Content.Contains(guid)));
			m_postman.UpdateExampleJSON(list.Content, "Rebellions", "List all rebellions");
		}*/
	}
}
