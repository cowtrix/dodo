using Dodo.SharedTest;
using Dodo.Sites;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Resources;
using System;
using System.Threading.Tasks;

namespace RESTTests
{
	[TestClass]
	public class MarchSiteTests : SiteTests<MarchSite>
	{
		[TestMethod]
		public async Task CanPatchStartDate()
		{
			GetRandomUser(out var pass, out var context);
			await Login(context.User.AuthData.Username, pass);
			var site = CreateObject<MarchSite>(context);
			var date = SchemaGenerator.RandomDate;
			await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{site.Guid}", EHTTPRequestType.PATCH, new { startDate = date });
			var updatedSite = ResourceManager.GetSingle(r => r.Guid == site.Guid) as MarchSite;
			Assert.IsTrue(date.ToUniversalTime() - updatedSite.StartDate.ToUniversalTime() < TimeSpan.FromMinutes(1));
		}

		[TestMethod]
		public async Task CanPatchEndDate()
		{
			GetRandomUser(out var pass, out var context);
			await Login(context.User.AuthData.Username, pass);
			var site = CreateObject<MarchSite>(context);
			var date = SchemaGenerator.RandomDate;
			await RequestJSON($"{DodoServer.DodoServer.API_ROOT}{ResourceRoot}/{site.Guid}", EHTTPRequestType.PATCH, new { endDate = date });
			var updatedSite = ResourceManager.GetSingle(r => r.Guid == site.Guid) as MarchSite;
			Assert.IsTrue(date.ToUniversalTime() - updatedSite.EndDate.ToUniversalTime() < TimeSpan.FromMinutes(1));
		}

		protected override JObject GetPatchObject()
		{
			var result = new JObject();
			result["startDate"] = new DateTime(2020, 6, 20).ToUniversalTime();
			result["endDate"] = new DateTime(2020, 6, 25).ToUniversalTime();
			result["publicDescription"] = "test 124";
			return result;
		}

		protected override void VerifyPatchedObject(MarchSite rsc, JObject patchObj)
		{
			Assert.IsTrue(rsc.StartDate.ToUniversalTime() - new DateTime(2020, 6, 20).ToUniversalTime() < TimeSpan.FromMinutes(1));
			Assert.IsTrue(rsc.EndDate.ToUniversalTime() - new DateTime(2020, 6, 25).ToUniversalTime() < TimeSpan.FromMinutes(1));
			Assert.AreEqual(rsc.PublicDescription, "test 124");
		}
	}
}
