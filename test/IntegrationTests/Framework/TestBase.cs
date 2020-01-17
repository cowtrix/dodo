using Common;
using Common.Extensions;
using Dodo;
using Dodo.Rebellions;
using Dodo.Users;
using DodoTest.Framework.Postman;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net;
using System.Net.Security;
using System.Security.Cryptography.X509Certificates;

namespace RESTTests
{
	public static class AssertX
	{
		public static void Throws<T>(Action action, Func<Exception, bool> exceptionValidator) where T:Exception
		{
			try
			{
				action();
			}
			catch(T e)
			{
				if(!exceptionValidator(e))
				{
					throw new AssertFailedException("Incorrect exception was thrown: " + e.Message);
				}
				return;
			}
			throw new AssertFailedException("Exception was not thrown");
		}
	}

	[TestClass]
	public abstract class TestBase
	{
		protected static TestContext Context;
		protected static RestClient RestClient = new RestClient("https://localhost:443");

		private Random m_random = new Random();
		
		protected static PostmanCollection m_postman = new PostmanCollection("8888079-57fb4f3e-b2ad-4afe-a429-47a38866c5cd");

		protected string DefaultUsername = "test_user";
		protected string DefaultName = "Test User";
		protected string DefaultPassword = "x@asjdbasjdas";
		protected string DefaultEmail = "test@web.com";
		protected string DefaultGUID;

		[AssemblyInitialize]
		public static void SetupTests(TestContext testContext)
		{
			Dodo.Dodo.Initialise();
			ResourceUtility.ClearAllManagers();
			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
			RestClient.PreAuthenticate = true;
			RestClient.Timeout = 500 * 1000;
			Context = testContext;
			Logger.OnLog += OnLog;
		}

		private static void OnLog(string message, ELogLevel logLevel)
		{
			Context.WriteLine(message);
		}

		public static bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			return true;
		}

		/// <summary>
		///  Gets or sets the test context which provides
		///  information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get { return Context; }
			set { Context = value; }
		}

		[TestCleanup]
		public void Clean()
		{
			ResourceUtility.ClearAllManagers();
		}

		[AssemblyCleanup]
		public static void Finalise()
		{
			m_postman.Update();
		}

		protected JObject RegisterUser(out string guid, string username = null, string name = null, string password = null, string email = null, bool verifyEmail = true)
		{
			username = username ?? DefaultUsername;
			name = name ?? DefaultName;
			password = password ?? DefaultPassword;
			email = email ?? DefaultEmail;
			var jobj = RequestJSON("register", Method.POST, new UserRESTHandler.CreationSchema(username, password, name, email), "", "");
			guid = jobj.Value<string>("GUID");
			if(verifyEmail)
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
			request.AddJsonBody(new RebellionRESTHandler.CreationSchema(name, "test description", new GeoLocation(66, 66), RebellionTests.DefaultStart, RebellionTests.DefaultEnd));
			var response = RestClient.Execute(request).Content;
			if(!response.IsValidJson())
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

		long LongRandom(long min, long max)
		{
			long result = m_random.Next((Int32)(min >> 32), (Int32)(max >> 32));
			result = (result << 32);
			result = result | (long)m_random.Next((Int32)min, (Int32)max);
			return result;
		}
	}
}
