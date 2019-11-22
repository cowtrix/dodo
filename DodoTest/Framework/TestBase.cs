﻿using Common;
using Dodo;
using Dodo.Rebellions;
using Dodo.Users;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
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
		private TestContext testContextInstance;
		private Random m_random = new Random();
		protected RestClient RestClient = new RestClient("https://localhost:443");

		protected string DefaultUsername = "test_user";
		protected string DefaultName = "Test User";
		protected string DefaultPassword = "x@asjdbasjdas";
		protected string DefaultEmail = "test@web.com";

		public TestBase()
		{
			DodoServer.Initialise();
			ServicePointManager.ServerCertificateValidationCallback = MyRemoteCertificateValidationCallback;
			RestClient.PreAuthenticate = true;
		}

		public bool MyRemoteCertificateValidationCallback(System.Object sender, X509Certificate certificate, X509Chain chain, SslPolicyErrors sslPolicyErrors)
		{
			bool isOk = true;
			// If there are errors in the certificate chain, look at each error to determine the cause.
			/*if (sslPolicyErrors != SslPolicyErrors.None)
			{
				for (int i = 0; i < chain.ChainStatus.Length; i++)
				{
					if (chain.ChainStatus[i].Status != X509ChainStatusFlags.RevocationStatusUnknown)
					{
						chain.ChainPolicy.RevocationFlag = X509RevocationFlag.EntireChain;
						chain.ChainPolicy.RevocationMode = X509RevocationMode.Online;
						chain.ChainPolicy.UrlRetrievalTimeout = new TimeSpan(0, 1, 0);
						chain.ChainPolicy.VerificationFlags = X509VerificationFlags.AllFlags;
						bool chainIsValid = chain.Build((X509Certificate2)certificate);
						if (!chainIsValid)
						{
							isOk = false;
						}
					}
				}
			}*/
			return isOk;
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

		protected JObject RegisterUser(string username = null, string name = null, string password = null, string email = null)
		{
			username = username ?? DefaultUsername;
			name = name ?? DefaultName;
			password = password ?? DefaultPassword;
			email = email ?? DefaultEmail;

			var request = new RestRequest("register", Method.POST);
			request.AddJsonBody(new { Username = username, Name = name, Password = password, Email = email });
			var response = RestClient.Execute(request).Content;
			if (!response.IsValidJson())
			{
				throw new Exception(response);
			}
			return JsonConvert.DeserializeObject<JObject>(response);
		}

		protected JObject RegisterRandomUser(out string username, out string name, out string password, out string email)
		{
			username = StringExtensions.RandomString(10).ToLower();
			name = StringExtensions.RandomString(10).ToLower();
			password = "@" + username;
			email = $"{StringExtensions.RandomString(5).ToLower()}@{StringExtensions.RandomString(5).ToLower()}.com";
			return RegisterUser(username, name, password, email);
		}

		protected JObject CreateNewRebellion(string name, GeoLocation location)
		{
			var request = new RestRequest("rebellions/create", Method.POST);
			AuthoriseRequest(request, DefaultUsername, DefaultPassword);
			request.AddJsonBody(new RebellionRESTHandler.CreationSchema { Name = name, Location = new GeoLocation(66, 66) });
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
				throw new Exception(content);
			}
			return JsonConvert.DeserializeObject<JObject>(content);
		}

		protected IRestResponse Request(string url, Method method, object data = null)
		{
			var request = new RestRequest(url, method);
			AuthoriseRequest(request, DefaultUsername, DefaultPassword);
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
