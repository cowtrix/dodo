using Dodo.LocalGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using DodoResources.LocalGroups;
using System.Threading.Tasks;
using Resources;
using Dodo.SharedTest;
using Dodo.Users;
using System.Linq;
using Newtonsoft.Json.Linq;
using Newtonsoft.Json;
using Resources.Location;

namespace RESTTests
{
	[TestClass]
	public class LocalGroupTests : GroupResourceTestBase<LocalGroup, LocalGroupSchema>
	{
		public override string ResourceRoot => LocalGroupAPIController.RootURL;

		protected override string PostmanCategory => "Local Groups";

		protected override JObject GetPatchObject()
		{
			var ret = new JObject();
			ret["Location"] = JToken.FromObject(SchemaGenerator.RandomLocation);
			ret["PublicDescription"] = "test test test";
			return ret;
		}

		protected override void VerifyPatchedObject(LocalGroup rsc, JObject patchObj)
		{
			Assert.AreEqual(patchObj["Location"].ToObject<GeoLocation>(), rsc.Location);
			Assert.AreEqual(patchObj.Value<string>("PublicDescription"), rsc.PublicDescription);
		}
	}
}
