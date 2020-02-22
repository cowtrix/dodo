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
using DodoResources;
using System.Collections.Generic;
using System.Linq;

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
				.UseStartup<DodoIdentity.IdentityStartup>());
			m_resourceServer = new TestServer(new WebHostBuilder()
				.UseStartup<DodoResources.ResourceStartup>());

			m_resourceClient = m_resourceServer.CreateClient();
			m_authClient = m_authServer.CreateClient();
		}

		protected async Task<JObject> RequestJSON(string url, EHTTPRequestType method, object data = null, IEnumerable<ValueTuple<string, string>> parameters = null)
		{
			var response = await Request(m_resourceClient, url, method, data, parameters);
			var content = await response.Content.ReadAsStringAsync();
			Assert.IsTrue(content.IsValidJson(),
				$"Invalid JSON: {response.StatusCode} | {response.ReasonPhrase} | {content}");
			return JsonConvert.DeserializeObject<JObject>(content);
		}

		protected async Task<T> RequestJSON<T>(string url, EHTTPRequestType method, object data = null, IEnumerable<ValueTuple<string, string>> parameters = null)
		{
			var response = await Request(m_resourceClient, url, method, data, parameters);
			var content = await response.Content.ReadAsStringAsync();
			Assert.IsTrue(content.IsValidJson(),
				$"Invalid JSON: {response.StatusCode} | {response.ReasonPhrase} | {content}");
			return JsonConvert.DeserializeObject<T>(content);
		}

		protected async Task<HttpResponseMessage> RequestAuth(string url, EHTTPRequestType method, object data = null)
		{
			return await Request(m_authClient, url, method, data);
		}

		protected async Task<HttpResponseMessage> RequestResource(string url, EHTTPRequestType method, object data = null)
		{
			return await Request(m_resourceClient, url, method, data);
		}

		private static async Task<HttpResponseMessage> Request(HttpClient client, string url, EHTTPRequestType method, object data = null, IEnumerable<ValueTuple<string, string>> parameters = null)
		{
			if (parameters != null && parameters.Any())
			{
				url += "?" + string.Join("&", parameters.Select(x => $"{Uri.EscapeUriString(x.Item1)}={Uri.EscapeUriString(x.Item2)}"));
			}
			HttpResponseMessage response;
			switch (method)
			{
				case EHTTPRequestType.GET:
					response = await client.GetAsync(url);
					break;
				case EHTTPRequestType.POST:
					response = await client.PostAsync(url, new StringContent(JsonConvert.SerializeObject(data), Encoding.UTF8, "application/json"));
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

		protected async Task Authorize(string username, string password, string url)
		{
			var disco = await m_authClient.GetDiscoveryDocumentAsync();
			Assert.IsFalse(disco.IsError, disco.Error);
			var tokenResponse = await m_authClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest
			{
				Address = disco.TokenEndpoint,
				ClientId = "spa",
				RequestUri = new Uri(m_resourceClient.BaseAddress + url),
				Code = "code", //?
				RedirectUri = "redirect",//?
			});
			if (tokenResponse.IsError)
			{
				throw new Exception(tokenResponse.Error);
			}
			m_resourceClient.SetBearerToken(tokenResponse.AccessToken);
			m_authClient.SetBearerToken(tokenResponse.AccessToken);

			/*
			var authorizeRequest = await m_authClient.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest()
			{
				RequestUri = new Uri(m_resourceClient.BaseAddress + url),
				ClientId = "spa",
			});
			if (authorizeRequest.IsError)
			{
				throw new Exception(tokenResponse.Error);
			}
			m_resourceClient.SetBearerToken(authorizeRequest.AccessToken);
			m_authClient.SetBearerToken(authorizeRequest.AccessToken);*/
		}
	}
}
