using System;
using System.Diagnostics;
using System.Linq;
using System.Net;
using System.Net.Http;
using System.Net.Security;
using System.Reflection;
using System.Security.Cryptography.X509Certificates;
using Common;
using Common.Extensions;
using Dodo;
using Dodo.Rebellions;
using Dodo.Users;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REST;
using RestSharp;
using SharedTest;

namespace RESTTests
{

	public abstract class RESTTestBase<T> : TestBase where T:DodoResource
	{
		public abstract string CreationURL { get; }
		public abstract object GetCreationSchema(bool unique = false);
		protected static RestClient RestClient;
		private readonly TestServer _server;
		private readonly HttpClient _client;

		public RESTTestBase()
		{
			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;

			_server = new TestServer(new WebHostBuilder()
				.UseStartup<DodoKubernetes.Startup>());
			_client = _server.CreateClient();

			RestClient = new RestClient(_client.BaseAddress);
			RestClient.PreAuthenticate = true;
			RestClient.Timeout = 500 * 1000;
		}

		public static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		[TestMethod]
		public virtual void CanCreate()
		{
			var sw = new Stopwatch();
			sw.Start();
			var createdObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			Assert.IsNotNull(createdObj.Value<string>("GUID"));
			CheckCreatedObject(createdObj);
			Context.WriteLine($"Test took {sw.Elapsed}");
		}

		[TestMethod]
		public virtual void CannotCreateDuplicate()
		{
			RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			AssertX.Throws<Exception>(() => RequestJSON(CreationURL, Method.POST, GetCreationSchema()),
				e => e.Message.Contains("Conflict"));
		}

		[TestMethod]
		public virtual void CannotPatchDuplicate()
		{
			var firstObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var secondObj = RequestJSON(CreationURL, Method.POST, GetCreationSchema(true));
			AssertX.Throws<Exception>(() =>
				RequestJSON(secondObj.Value<string>("ResourceURL"), Method.PATCH, new { Name = firstObj.Value<string>("Name") }),
				e => e.Message.Contains("Conflict - resource may already exist"));
			RequestJSON(secondObj.Value<string>("ResourceURL"), Method.GET);
		}

		protected virtual void CheckCreatedObject(JObject obj) { }

		[TestMethod]
		public virtual void CanGet()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceObj = RequestJSON(obj.Value<string>("ResourceURL"), Method.GET);
			Assert.IsNotNull(resourceObj.Value<string>("GUID"));
			CheckGetObject(resourceObj);
		}

		protected virtual void CheckGetObject(JObject obj) { }

		[TestMethod]
		public virtual void CanDestroy()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var response = Request(obj.Value<string>("ResourceURL"), Method.DELETE);
			Assert.IsTrue(response.StatusDescription.Contains("Resource deleted"));
		}

		[TestMethod]
		public virtual void CanGetByResource()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var resourceObj = RequestJSON("resources/" + obj.Value<string>("GUID"), Method.GET);
			Assert.AreEqual(resourceObj.Value<string>("GUID"), obj.Value<string>("GUID"));
		}

		public abstract object GetPatchSchema();
		[TestMethod]
		public virtual void CanPatch()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			var patch = RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, GetPatchSchema());
			Assert.AreNotEqual(obj.ToString(), patch.ToString());
			CheckPatchedObject(patch);
		}
		protected virtual void CheckPatchedObject(JObject obj) { }

		[TestMethod]
		public virtual void CannotPatchInvalid()
		{
			var obj = RequestJSON(CreationURL, Method.POST, GetCreationSchema());
			AssertX.Throws<Exception>(() => RequestJSON(obj.Value<string>("ResourceURL"), Method.PATCH, new { FakeField = "Not a field" }),
				x => x.Message.Contains("Invalid field names"));
		}


		protected JObject RegisterUser(out string guid, string username = null, string name = null, string password = null, string email = null, bool verifyEmail = true)
		{
			username = username ?? DefaultUsername;
			name = name ?? DefaultName;
			password = password ?? DefaultPassword;
			email = email ?? DefaultEmail;
			var jobj = RequestJSON("register", Method.POST, new UserSchema(default, username, password, name, email), "", "");
			guid = jobj.Value<string>("GUID");
			if (verifyEmail)
				VerifyUser(guid, username, password, email);
			return jobj;
		}

		protected IRestResponse VerifyUser(string guid, string username, string password, string email)
		{
			var verifyAction = ResourceUtility.GetManager<User>().GetSingle(u => u.WebAuth.Username == username)
				.PushActions.GetSinglePushAction<VerifyEmailAction>();
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
		}

		protected JObject RequestJSON(string url, Method method, object data = null, string user = null, string password = null)
		{
			var request = new RestRequest(url, method);
			AuthoriseRequest(request, user ?? DefaultUsername, password ?? DefaultPassword);
			if (data != null)
			{
				request.AddJsonBody(data);
			}
			var response = RestClient.Execute(request);
			var content = response.Content;
			if (!content.IsValidJson())
			{
				throw new Exception($"{response.StatusCode} | {response.StatusDescription} | {response.ResponseStatus} | {response.Content}");
			}
			return JsonConvert.DeserializeObject<JObject>(content);
		}

		protected IRestResponse Request(string url, Method method, object data = null, string username = null, string password = null)
		{
			var request = new RestRequest(url, method);
			AuthoriseRequest(request, username ?? DefaultUsername, password ?? DefaultPassword);
			if (data != null)
			{
				request.AddJsonBody(data);
			}
			return RestClient.Execute(request);
		}

		protected static void AuthoriseRequest(RestRequest request, string user, string password)
		{
			request.AddHeader("Authorization", "Basic " + StringExtensions.Base64Encode($"{user}:{password}"));
		}
	}
}
