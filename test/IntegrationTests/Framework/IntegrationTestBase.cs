using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using Resources;
using SharedTest;
using Common.Extensions;
using System.Text;
using System.Collections.Generic;
using System.Linq;
using Dodo.Users;
using System.Net;
using Dodo.Models;
using DodoServer;

namespace RESTTests
{
	public abstract class IntegrationTestBase : TestBase
	{
		protected string URL => Dodo.Dodo.NetConfig.FullURI;

		private readonly TestServer m_server;
		private readonly HttpClient m_client;
		protected CookieContainer m_cookies;

		protected HttpResponseMessage LastRequest { get; private set; }

		public IntegrationTestBase()
		{
			m_cookies = new CookieContainer();	
			m_server = new TestServer(new WebHostBuilder()
				.UseStartup<DodoStartup>()
				.UseUrls($"{Dodo.Dodo.NetConfig.Domain}:{Dodo.Dodo.NetConfig.SSLPort}"));

			m_client = m_server.CreateClient();
			m_client.BaseAddress = new Uri(m_client.BaseAddress.ToString().Replace("http", "https"));
		}

		protected async Task<JObject> RequestJSON(string url, EHTTPRequestType method, object data = null, 
			IEnumerable<ValueTuple<string, string>> parameters = null,
			Func<HttpResponseMessage, bool> validator = null)
		{
			return await RequestJSON<JObject>(url, method, data, parameters, validator);
		}

		protected async Task<T> RequestJSON<T>(string url, EHTTPRequestType method, object data = null, 
			IEnumerable<ValueTuple<string, string>> parameters = null,
			Func<HttpResponseMessage, bool> validator = null)
		{
			var response = await Request(url, method, data, parameters, validator);
			var content = await response.Content.ReadAsStringAsync();
			Assert.IsTrue(content.IsValidJson(),
				$"Invalid JSON: {response.StatusCode} | {response.ReasonPhrase} | {content}");
			return JsonConvert.DeserializeObject<T>(content);
		}

		protected async Task<HttpResponseMessage> Request(
			string url,
			EHTTPRequestType method,
			object data = null,
			IEnumerable<ValueTuple<string, string>> parameters = null,
			Func<HttpResponseMessage, bool> validator = null
			)
		{
			if (parameters != null && parameters.Any())
			{
				url += "?" + string.Join("&", parameters.Select(x => $"{Uri.EscapeUriString(x.Item1)}={Uri.EscapeUriString(x.Item2)}"));
			}
			LastRequest = null;
			HttpResponseMessage response;
			switch (method)
			{
				case EHTTPRequestType.GET:
					response = await m_client.GetAsync(url);
					break;
				case EHTTPRequestType.POST:
					response = await m_client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
					break;
				case EHTTPRequestType.PATCH:
					response = await m_client.PatchAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
					break;
				case EHTTPRequestType.DELETE:
					response = await m_client.DeleteAsync(url);
					break;
				default:
					throw new Exception("Unsupported method " + method);
			}
			LastRequest = response;
			validator ??= ((r) => r.IsSuccessStatusCode);
			if (!validator(response))
			{
				throw new Exception(response.ToString());
			}
			return response;
		}

		[AssemblyCleanup]
		public static void FinaliseIntegrationTestBase()
		{
			Finalise();
		}

		protected async Task Login(string username, string password)
		{
			var response = await m_client.PostAsync($"{UserService.RootURL}/{UserService.LOGIN}",
				new StringContent(JsonConvert.SerializeObject(new LoginModel { Username = username, Password = password }), 
				Encoding.UTF8, "application/json"));
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception(response.ToString());
			}
			var cookie = response.Headers.GetValues("Set-Cookie");
			m_client.DefaultRequestHeaders.Add("cookie", cookie);
			LastRequest = response;
		}

		protected async Task Logout()
		{
			var response = await m_client.GetAsync($"{UserService.RootURL}/{UserService.LOGOUT}");
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception(response.ToString());
			}
			m_client.DefaultRequestHeaders.Clear();
			LastRequest = response;
		}
	}
}
