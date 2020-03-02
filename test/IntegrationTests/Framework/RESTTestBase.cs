using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using System.Threading.Tasks;
using Common;
using Common.Extensions;
using Dodo;
using Dodo.Rebellions;
using Dodo.SharedTest;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources;

namespace RESTTests
{

	public abstract class RESTTestBase<T> : IntegrationTestBase where T:DodoResource
	{
		public abstract string ResourceRoot { get; }
		
		[TestMethod]
		public async virtual Task CanGetAnonymously()
		{
			GetRandomUser(out _, out var context);
			var resource = ResourceUtility.GetFactory<T>().CreateTypedObject(context, SchemaGenerator.GetRandomSchema<T>(context));
			var resourceObj = await RequestJSON($"{ResourceRoot}/{resource.GUID.ToString()}", EHTTPRequestType.GET);
			VerifyCreatedObject(resource, resourceObj);
		}

		protected virtual void VerifyCreatedObject(T rsc, JObject obj)
		{
			Assert.AreEqual(rsc.GUID, Guid.Parse(obj.Value<string>("GUID")));
			Assert.AreEqual(rsc.Name, obj.Value<string>("Name"));
		}

		[TestMethod]
		public virtual async Task CanCreate()
		{
			var user = GetRandomUser(out var password, out var context);
			await Login(user.AuthData.Username, password);
			var response = await RequestJSON(ResourceRoot, EHTTPRequestType.POST,
				SchemaGenerator.GetRandomSchema<T>(context));
		}

		protected virtual T CreateObject(AccessContext context = default)
		{
			if(context.User == null)
			{
				GetRandomUser(out var password, out context); ;
			}
			var schema = SchemaGenerator.GetRandomSchema<T>(context);
			var factory = ResourceUtility.GetFactory<T>();
			return factory.CreateTypedObject(context, schema);
		}

		/*[TestMethod]
		public async virtual void CanCreate()
		{
			var sw = new Stopwatch();
			sw.Start();
			var createdObj = await RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			Assert.IsNotNull(createdObj.Value<string>("GUID"));
			CheckCreatedObject(createdObj);
			Context.WriteLine($"Test took {sw.Elapsed}");
		}

		[TestMethod]
		public async virtual void CannotCreateDuplicate()
		{
			await RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			AssertX.Throws<Exception>(() => await RequestJSON(CreationURL, Method.POST, GetCreationSchema()),
				e => e.Message.Contains("Conflict"));
		}

		[TestMethod]
		public async virtual void CannotPatchDuplicate()
		{
			var firstObj = await RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var secondObj = await RequestJSON(CreationURL, Method.POST, GetCreationSchema(true));
			AssertX.Throws<Exception>(async () =>
				await await RequestJSON(secondObj.Value<string>("ResourceURL"), Method.PATCH, new { Name = firstObj.Value<string>("Name") }),
				e => e.Message.Contains("Conflict - resource may already exist"));
			await RequestJSON(secondObj.Value<string>("ResourceURL"), Method.GET);
		}

		protected virtual void CheckCreatedObject(JObject obj) { }

		

		protected virtual void CheckGetObject(JObject obj) { }

		[TestMethod]
		public async virtual void CanDestroy()
		{
			var obj = await RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var response = Request(obj.Value<string>("ResourceURL"), Method.DELETE);
			Assert.IsTrue(response.StatusDescription.Contains("Resource deleted"));
		}

		[TestMethod]
		public async virtual void CanGetByResource()
		{
			var obj = await RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceObj = await RequestJSON("resources/" + obj.Value<string>("GUID"), Method.GET);
			Assert.AreEqual(resourceObj.Value<string>("GUID"), obj.Value<string>("GUID"));
		}

		public abstract object GetPatchSchema();

		[TestMethod]
		public async virtual void CanPatch()
		{
			var obj = await RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var patch = await RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, GetPatchSchema());
			Assert.AreNotEqual(obj.ToString(), patch.ToString());
			CheckPatchedObject(patch);
		}
		protected virtual void CheckPatchedObject(JObject obj) { }

		[TestMethod]
		public async virtual void CannotPatchInvalid()
		{
			var obj = await RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			AssertX.Throws<Exception>(async () => await RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, new { FakeField = "Not a field" }),
				x => x.Message.Contains("Invalid field names"));
		}


		protected async Task<JObject> RegisterUser(out string guid, string username = null, string name = null, string password = null, string email = null, bool verifyEmail = true)
		{
			username = username ?? DefaultUsername;
			name = name ?? DefaultName;
			password = password ?? DefaultPassword;
			email = email ?? DefaultEmail;
			var jobj = await RequestJSON("register", Method.POST, new UserSchema(default, username, password, name, email), "", "");
			guid = jobj.Value<string>("GUID");
			if (verifyEmail)
				VerifyUser(guid, username, password, email);
			return jobj;
		}

		protected IRestResponse VerifyUser(string guid, string username, string password, string email)
		{
			var verifyAction = ResourceUtility.GetManager<User>().GetSingle(u => u.WebAuth.Username == username)
				.Tokens.GetSingleToken<VerifyEmailAction>();
			var response = Request($"verify?token={verifyAction.Token}", Method.POST, null, username, password);
			Assert.IsTrue(response.Content.Contains("Email verified"), response + response.Content);
			return response;
		}

		protected JObject RegisterRandomUser(out string username, out string name, out string password, out string email, out string guid, bool verifyEmail = true)
		{
			username = StringExtensions.RandomString(10).ToLower();
			name = StringExtensions.RandomString(10).ToLower();
			password = "@" + username;
			email = $"{StringExtensions.RandomString(5).ToLower()}@{StringExtensions.RandomString(5).ToLower()}.com";
			return RegisterUser(out guid, username, name, password, email, verifyEmail);
		}

		protected JObject CreateNewRebellion(string name, GeoLocation location)
		{
			var request = new RestRequest("rebellions/create", Method.POST);
			AuthoriseRequest(request, DefaultUsername, DefaultPassword);
			request.AddJsonBody(new RebellionSchema(name, "test description", new GeoLocation(66, 66), RebellionTests.DefaultStart, RebellionTests.DefaultEnd));
			var response = RestClient.Execute(request).Content;
			if (!response.IsValidJson())
			{
				throw new Exception(response);
			}
			return JsonConvert.DeserializeObject<JObject>(response);
		}

		protected JObject PatchObject<T>(string url, T anonObj)
		{
			var request = new RestRequest(url, Method.PATCH);
			AuthoriseRequest(request, DefaultUsername, DefaultPassword);
			request.AddJsonBody(anonObj);
			var response = RestClient.Execute(request).Content;
			if (!response.IsValidJson())
			{
				throw new Exception(response);
			}
			return JsonConvert.DeserializeObject<JObject>(response);
		}

		protected JObject GetResource(string url)
		{
			var request = new RestRequest("resources/" + url, Method.GET);
			AuthoriseRequest(request, DefaultUsername, DefaultPassword);
			var response = RestClient.Execute(request).Content;
			if (!response.IsValidJson())
			{
				throw new Exception(response);
			}
			return JsonConvert.DeserializeObject<JObject>(response);
		}*/



		/*protected static void AuthoriseRequest(RestRequest request, string user, string password)
		{
			request.AddHeader("Authorization", "Basic " + StringExtensions.Base64Encode($"{user}:{password}"));
		}*/
	}
}
