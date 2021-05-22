using Dodo;
using Dodo.RoleApplications;
using Dodo.Roles;
using Dodo.Roles;
using DodoTest.Framework.Postman;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using Resources;
using System;
using System.Threading.Tasks;

namespace RESTTests
{
	[TestClass]
	public class RoleTests : RESTTestBase<Role, RoleSchema>
	{
		public override string ResourceRoot => RoleAPIController.RootURL;

		protected override string PostmanCategory => "Roles";

		protected override JObject GetPatchObject()
		{
			var ret = new JObject();
			ret["PublicDescription"] = "test test test";
			return ret;
		}

		protected override void VerifyPatchedObject(Role rsc, JObject patchObj)
		{
			Assert.AreEqual(patchObj.Value<string>("PublicDescription"), rsc.PublicDescription);
		}

		[TestMethod]
		[TestCategory("Joining & Leaving")]
		public async Task CanApply()
		{
			var group = CreateObject<Role>();
			var user = GetRandomUser(out var password, out var context);
			await Login(user.Slug, password);
			var joinReq = await Request($"{Dodo.DodoApp.API_ROOT}{RoleApplicationAPIController.RootURL}/{group.Guid}/apply", EHTTPRequestType.POST,
				"This is my test application", 
				validator: m => m.StatusCode == System.Net.HttpStatusCode.Redirect);
			var verify = await RequestJSON($"{Dodo.DodoApp.API_ROOT}{ResourceRoot}/{group.Guid}", EHTTPRequestType.GET);
			Assert.AreNotEqual(verify[Resource.METADATA]["applied"].Value<string>(), default(System.Guid).ToString());
			Postman.Update(
				new PostmanEntryAddress { Category = PostmanCategory, Request = $"Apply for a Role" },
				joinReq);
		}
	}
}
