using Dodo;
using Dodo.SharedTest;
using Dodo.Sites;
using DodoResources.Sites;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;
using Common.Extensions;

namespace RESTTests
{
	public abstract class SiteTests<T> : RESTTestBase<T, SiteSchema> where T:Site
	{
		public override string ResourceRoot => SiteController.RootURL;
	}

	public class EventSiteTests : SiteTests<EventSite>
	{
		protected override string PostmanCategory => throw new NotImplementedException();

		[TestMethod]
		public async Task CanPatchStartDate()
		{
			GetRandomUser(out _, out var context);
			var site = CreateObject<EventSite>(context);
			var date = SchemaGenerator.RandomDate;
			await RequestJSON($"{ResourceRoot}/{site.Guid}", EHTTPRequestType.POST, new { startDate = date });
			var updatedSite = ResourceManager.GetSingle(r => r.Guid == site.Guid) as EventSite;
			Assert.AreEqual(date, updatedSite.StartDate);
		}

		protected override JObject GetPatchObject()
		{
			var result = new JObject();
			result["startDate"] = new DateTime(2020, 6, 20);
			result["endDate"] = new DateTime(2020, 6, 25);
			result["publicDescription"] = "test 124";
			return result;
		}

		protected override void VerifyPatchedObject(EventSite rsc, JObject patchObj)
		{
			Assert.AreEqual(rsc.StartDate, new DateTime(2020, 6, 20));
			Assert.AreEqual(rsc.EndDate, new DateTime(2020, 6, 25));
			Assert.AreEqual(rsc.PublicDescription, "test 124");
		}
	}
}
