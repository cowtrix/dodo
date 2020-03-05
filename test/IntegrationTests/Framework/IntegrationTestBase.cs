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
using IdentityModel.Client;
using System.Collections.Generic;
using System.Linq;
using Dodo.Users;
using Dodo;
using System.Net;
using static IdentityModel.OidcConstants;
using Resources.Security;
using Common.Security;
using IdentityModel;
using DodoServer;

namespace RESTTests
{
	public abstract class IntegrationTestBase : TestBase
	{
		protected string URL => DodoServer.DodoServer.HttpsUrl;

		private readonly TestServer m_server;
		private readonly HttpClient m_client;
		protected CookieContainer m_cookies;

		public IntegrationTestBase()
		{
			m_cookies = new CookieContainer();	
			m_server = new TestServer(new WebHostBuilder()
				.UseStartup<DodoServer.DodoStartup>()
				.UseUrls(DodoServer.DodoServer.HttpsUrl));

			m_client = m_server.CreateClient();
			m_client.BaseAddress = new Uri(m_client.BaseAddress.ToString().Replace("http", "https"));
		}

		protected async Task<JObject> RequestJSON(string url, EHTTPRequestType method, object data = null, IEnumerable<ValueTuple<string, string>> parameters = null)
		{
			return await RequestJSON<JObject>(url, method, data, parameters);
		}

		protected async Task<T> RequestJSON<T>(string url, EHTTPRequestType method, object data = null, IEnumerable<ValueTuple<string, string>> parameters = null)
		{
			var response = await Request(url, method, data, parameters);
			var content = await response.Content.ReadAsStringAsync();
			Assert.IsTrue(content.IsValidJson(),
				$"Invalid JSON: {response.StatusCode} | {response.ReasonPhrase} | {content}");
			return JsonConvert.DeserializeObject<T>(content);
		}

		protected async Task<HttpResponseMessage> Request(string url, EHTTPRequestType method, object data = null, IEnumerable<ValueTuple<string, string>> parameters = null)
		{
			if (parameters != null && parameters.Any())
			{
				url += "?" + string.Join("&", parameters.Select(x => $"{Uri.EscapeUriString(x.Item1)}={Uri.EscapeUriString(x.Item2)}"));
			}
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
				default:
					throw new Exception("Unsupported method " + method);
			}
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception(response.ToString());
			}
			return response;
		}

		protected async Task Login(string username, string password)
		{
			var response = await m_client.PostAsync($"{UserController.RootURL}/{UserController.LOGIN}",
				new StringContent(JsonConvert.SerializeObject(new UserController.LoginModel { username = username, password = password }), 
				Encoding.UTF8, "application/json"));
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception(response.ToString());
			}
			var cookie = response.Headers.GetValues("Set-Cookie");
			m_client.DefaultRequestHeaders.Add("cookie", cookie);
		}

		protected async Task Logout()
		{
			var response = await m_client.GetAsync($"{UserController.RootURL}/{UserController.LOGOUT}");
			if (!response.IsSuccessStatusCode)
			{
				throw new Exception(response.ToString());
			}
			m_client.DefaultRequestHeaders.Clear();
		}
	}
}
