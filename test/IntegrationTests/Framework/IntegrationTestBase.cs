using System;
using System.Net.Http;
using System.Threading.Tasks;
using Microsoft.AspNetCore.Hosting;
using Microsoft.AspNetCore.TestHost;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using REST;
using SharedTest;
using Common.Extensions;
using System.Text;
using IdentityModel.Client;

namespace RESTTests
{
	public abstract class IntegrationTestBase : TestBase
	{
		private readonly TestServer m_resourceServer;
		private readonly TestServer m_authServer;
		private readonly HttpClient m_resourceClient;
		private readonly HttpClient m_authClient;

		public IntegrationTestBase()
		{
			m_authServer = new TestServer(new WebHostBuilder()
				.UseStartup<DodoIdentity.Startup>());
			m_resourceServer = new TestServer(new WebHostBuilder()
				.UseStartup<DodoKubernetes.Startup>());
			m_resourceClient = m_resourceServer.CreateClient();
			m_authClient = m_authServer.CreateClient();
		}

		protected async Task<JObject> RequestJSON(string url, EHTTPRequestType method, object data = null)
		{
			var response = await Request(m_resourceClient, url, method, data);
			var content = await response.Content.ReadAsStringAsync();
			Assert.IsTrue(content.IsValidJson(),
				$"Invalid JSON: {response.StatusCode} | {response.ReasonPhrase} | {content}");
			return JsonConvert.DeserializeObject<JObject>(content);
		}

		protected async Task<HttpResponseMessage> RequestAuth(string url, EHTTPRequestType method, object data = null)
		{
			return await Request(m_authClient, url, method, data);
		}

		private static async Task<HttpResponseMessage> Request(HttpClient client, string url, EHTTPRequestType method, object data = null)
		{
			HttpResponseMessage response;
			switch (method)
			{
				case EHTTPRequestType.GET:
					response = await client.GetAsync(url);
					break;
				case EHTTPRequestType.POST:
					response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8,
									"application/json"));
					break;
				default:
					throw new Exception("Unsupported method " + method);
			}
			return response;
		}

		protected async Task Authorize(string url, string username, string password)
		{
			var disco = await m_authClient.GetDiscoveryDocumentAsync();
			Assert.IsFalse(disco.IsError, disco.Error);

			var tokenResponse = await m_authClient.RequestPasswordTokenAsync(new PasswordTokenRequest
			{
				Address = url,
				UserName = username,
				Password = password,
			});
			Assert.IsFalse(tokenResponse.IsError, tokenResponse.Error);


		}
	}
}
