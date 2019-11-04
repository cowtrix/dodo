using Common;
using Dodo;
using Dodo.Rebellions;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;

namespace DodoTest
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
		private TestContext testContextInstance;
		private Random m_random = new Random();
		protected RestClient RestClient = new RestClient("http://localhost:8080");

		protected string CurrentLogin = "TestUser";
		protected string CurrentPassword = "password";

		public TestBase()
		{
			DodoServer.Initialise();
		}

		/// <summary>
		///  Gets or sets the test context which provides
		///  information about and functionality for the current test run.
		///</summary>
		public TestContext TestContext
		{
			get { return testContextInstance; }
			set { testContextInstance = value; }
		}

		[TestCleanup]
		public void Clean()
		{
			DodoServer.CleanAllData();
		}

		protected JObject RegisterUser(string username, string password)
		{
			var request = new RestRequest("register", Method.POST);
			request.AddJsonBody(new { Username = username, Password = password });
			var response = RestClient.Execute(request);
			return JsonConvert.DeserializeObject<JObject>(response.Content);
		}

		protected JObject CreateNewRebellion(string name, GeoLocation location)
		{
			var request = new RestRequest("newrebellion", Method.POST);
			request.AddHeader("user", CurrentLogin);
			request.AddHeader("token", CurrentPassword);
			request.AddJsonBody(new { RebellionName = name, Location = new GeoLocation(66, 66) });
			var response = RestClient.Execute(request).Content;
			if(!response.IsValidJson())
			{
				throw new Exception(response);
			}
			return JsonConvert.DeserializeObject<JObject>(response);
		}

		protected JObject PatchObject<T>(string url, T anonObj)
		{
			var request = new RestRequest("resource/" + url, Method.PATCH);
			request.AddHeader("user", CurrentLogin);
			request.AddHeader("token", CurrentPassword);
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
			var request = new RestRequest("resource/" + url, Method.GET);
			request.AddHeader("user", CurrentLogin);
			request.AddHeader("token", CurrentPassword);
			var response = RestClient.Execute(request).Content;
			if (!response.IsValidJson())
			{
				throw new Exception(response);
			}
			return JsonConvert.DeserializeObject<JObject>(response);
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
