using Dodo;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;

namespace DodoTest
{

	[TestClass]
	public abstract class TestBase
	{
		private TestContext testContextInstance;
		private Random m_random = new Random();
		protected RestClient RestClient = new RestClient("http://localhost:8080");

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
		}

		protected JObject RegisterUser(string username, string password)
		{
			var request = new RestRequest("register", Method.POST);
			request.AddJsonBody(new { Username = username, Password = password });
			var response = RestClient.Execute(request);
			return JsonConvert.DeserializeObject<JObject>(response.Content);
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
