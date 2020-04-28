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
using Dodo.WorkingGroups;
using DodoTest.Framework.Postman;
using Resources.Location;

namespace RESTTests
{
	[TestClass]
	public class RebellionTests : GroupResourceTestBase<Rebellion, RebellionSchema>
	{
		public static DateTime DefaultStart => new DateTime(2019, 10, 7, 0, 0, 0, DateTimeKind.Utc);
		public static DateTime DefaultEnd => new DateTime(2019, 10, 14, 0, 0, 0, DateTimeKind.Utc);
		public override string ResourceRoot => RebellionController.RootURL;
		protected override string PostmanCategory => "Rebellions";

		protected override void VerifyCreatedObject(Rebellion rebellion, JObject obj, RebellionSchema schema)
		{
			base.VerifyCreatedObject(rebellion, obj, schema);
			Assert.AreEqual(schema.Location, rebellion.Location);
			Assert.IsTrue(schema.StartDate.ToUniversalTime() - rebellion.StartDate.ToUniversalTime() < TimeSpan.FromMinutes(2), $"Start date wasn't close enough - delta was {schema.StartDate - rebellion.StartDate}");
			Assert.IsTrue(schema.EndDate.ToUniversalTime() - rebellion.EndDate.ToUniversalTime() < TimeSpan.FromMinutes(2), $"End date wasn't close enough - delta was {schema.EndDate - rebellion.EndDate}");
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
			Assert.IsTrue(patchObj.Value<DateTime>("StartDate").ToUniversalTime() - rsc.StartDate.ToUniversalTime() < TimeSpan.FromMinutes(2));
			Assert.IsTrue(patchObj.Value<DateTime>("EndDate").ToUniversalTime() - rsc.EndDate.ToUniversalTime() < TimeSpan.FromMinutes(2));
			Assert.AreEqual(patchObj.Value<string>("PublicDescription"), rsc.PublicDescription);
		}

		[TestMethod]
		public async Task PatchRebellionWithWorkingGroups()
		{
			var user = GetRandomUser(out var password, out var context);
			var rebellion = CreateObject<Rebellion>(context);
			var wg = CreateObject<WorkingGroup>(context, new WorkingGroupSchema("Test WG", "test", rebellion.Guid));

			await Login(user.AuthData.Username, password);
			var patch = GetPatchObject();
			await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{rebellion.Guid}", EHTTPRequestType.PATCH, patch);
			var updatedObj = ResourceManager.GetSingle(r => r.Guid == rebellion.Guid);
			VerifyPatchedObject(updatedObj, patch);
		}
	}
}
