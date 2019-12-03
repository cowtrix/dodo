using Common.Extensions;
using Dodo;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json.Linq;
using RestSharp;
using SimpleHttpServer.REST;
using System;
using System.Linq;

namespace RESTTests
{
	public abstract class GroupResourceTestBase<T> : RESTTestBase<T> where T:GroupResource
	{
		[TestMethod]
		public void CanAddAdminFromExistingUser()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceURL = createdObj.Value<string>("ResourceURL");
			createdObj = RequestJSON(resourceURL, Method.GET);
			var adminBefore = createdObj.Value<JArray>("Administrators").AsJEnumerable().Select(x => x.Value<string>("Guid"));
			Assert.IsTrue(adminBefore.All(x => x == DefaultGUID));
			RegisterRandomUser(out var username1, out _, out var password, out _, out var guid);
			var addAdminResponse = Request(resourceURL + GroupResourceRESTHandler<T>.ADD_ADMIN, Method.POST, guid);
			Assert.IsTrue(addAdminResponse.StatusCode == System.Net.HttpStatusCode.OK);

			var updatedObj = RequestJSON(resourceURL, Method.GET, user: username1, password:password);
			Assert.AreEqual("ADMIN", updatedObj.Value<string>(JsonViewUtility.PERMISSION_KEY));
			var adminAfter = updatedObj.Value<JArray>("Administrators").AsJEnumerable().Select(x => x.Value<string>("Guid"));
			Assert.IsNotNull(adminAfter.SingleOrDefault(x => x == guid));
		}

		[TestMethod]
		public void CanAddAdminFromNewUser()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceURL = createdObj.Value<string>("ResourceURL");

			var secondEmail = "test@blahnotawebsite.com.ul";
			var addAdminResponse = Request(resourceURL + GroupResourceRESTHandler<T>.ADD_ADMIN, Method.POST, secondEmail);
			Assert.IsTrue(addAdminResponse.StatusCode == System.Net.HttpStatusCode.OK);

			var secondPass = ValidationExtensions.GenerateStrongPassword();
			var secondUser = RegisterUser(out var guid, username: "seconduser", email: secondEmail, password: secondPass);
			createdObj = RequestJSON(resourceURL, Method.GET);
			var admin = createdObj.Value<JArray>("Administrators").AsJEnumerable().Select(x => x.Value<string>("Guid"));
			Assert.IsNotNull(admin.SingleOrDefault(x => x == guid));
		}

		[TestMethod]
		public void CannotEditIfNotAdmin()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceURL = createdObj.Value<string>("ResourceURL");

			RegisterRandomUser(out var username1, out _, out var password, out _, out var guid);

			AssertX.Throws<Exception>(() => RequestJSON(resourceURL, Method.PATCH, GetPatchSchema(), username1, password),
				e => e.Message.Contains("Forbidden"));

			AssertX.Throws<Exception>(() => RequestJSON(resourceURL, Method.DELETE, GetPatchSchema(), username1, password),
				e => e.Message.Contains("Forbidden"));
		}

		[TestMethod]
		public void CanGetIfNotAdmin()
		{
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceURL = createdObj.Value<string>("ResourceURL");

			var response = RequestJSON(resourceURL, Method.GET, null, "", "");

			RegisterRandomUser(out var username1, out _, out var password, out _, out var guid);
			response = RequestJSON(resourceURL, Method.GET, null, username1, password);
		}
	}
}
