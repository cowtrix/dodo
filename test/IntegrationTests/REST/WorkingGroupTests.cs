using Dodo.WorkingGroups;
using Dodo.WorkingGroups;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;

namespace RESTTests
{
	[TestClass]
	public class WorkingGroupTests : AdministratedGroupResourceTestBase<WorkingGroup, WorkingGroupSchema>
	{
		public override string ResourceRoot => WorkingGroupController.RootURL;

		protected override string PostmanCategory => "Working Groups";

		protected override JObject GetPatchObject()
		{
			var ret = new JObject();
			ret["PublicDescription"] = "test test test";
			return ret;
		}

		protected override void VerifyPatchedObject(WorkingGroup rsc, JObject patchObj)
		{
			Assert.AreEqual(patchObj.Value<string>("PublicDescription"), rsc.PublicDescription);
			//Postman.UpdateExampleJSON(patchObj.ToString(), "Working Groups", "Update a Working Group");
		}

		protected override void VerifyCreatedObject(WorkingGroup rsc, JObject obj, WorkingGroupSchema schema)
		{
			base.VerifyCreatedObject(rsc, obj, schema);
			//Postman.UpdateExampleJSON(obj.ToString(), "Working Groups", "Create a new Working Group");
		}
	}
}
