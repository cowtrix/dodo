using Dodo;
using Dodo.LocationResources;
using Dodo.Sites;
using System.Collections.Generic;
using System.Linq;
using Common.Extensions;
using Resources;
using Newtonsoft.Json.Linq;
using Dodo.SharedTest;
using Microsoft.VisualStudio.TestTools.UnitTesting;

namespace RESTTests
{
	[TestClass]
	public class SiteTests : GroupResourceTestBase<Site, LocationResourceSchema>
	{
		public override string ResourceRoot => SiteAPIController.RootURL;

		protected override string PostmanCategory => "Sites";

		protected override JObject GetPatchObject()
		{
			var result = new JObject();
			//result["facilities"] = new {  }
			result["publicDescription"] = "test 124";
			result["videoEmbedURL"] = "https://www.youtube.com/embed/d4QDM_Isi24";
			return result;
		}

		protected override void VerifyPatchedObject(Site rsc, JObject patchObj)
		{
			Assert.AreEqual(rsc.VideoEmbedURL, "https://www.youtube.com/embed/d4QDM_Isi24");
			Assert.AreEqual(rsc.PublicDescription, "test 124");
		}
	}
}
