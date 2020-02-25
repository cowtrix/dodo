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
				.UseStartup<DodoServer.DodoStartup>());

			m_client = m_server.CreateClient();
		}

		protected async Task<JObject> RequestJSON(string url, EHTTPRequestType method, object data = null, IEnumerable<ValueTuple<string, string>> parameters = null)
		{
			var response = await Request(url, method, data, parameters);
			var content = await response.Content.ReadAsStringAsync();
			Assert.IsTrue(content.IsValidJson(),
				$"Invalid JSON: {response.StatusCode} | {response.ReasonPhrase} | {content}");
			return JsonConvert.DeserializeObject<JObject>(content);
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

		protected async Task<string> Authorize(string username, string password, string url)
		{
			var disco = await m_client.GetDiscoveryDocumentAsync();
			Assert.IsFalse(disco.IsError, disco.Error);

			var scope = "api1";
			var audience = m_client.BaseAddress;
			var responsetype = "code";
			var clientId = "spa";
			url = URL;

			/*var fullUri = $"{disco.AuthorizeEndpoint}?scope={scope}&audience={audience}&response_type={responsetype}&client_id={clientId}&redirect_uri={url}&code=test";

			var response = await m_authClient.GetAsync(fullUri);*/
			var response = await m_client.RequestAuthorizationCodeTokenAsync(new AuthorizationCodeTokenRequest()
			{
				Address = disco.AuthorizeEndpoint,
				ClientId = clientId,
				Code = KeyGenerator.GetUniqueKey(128),
				RedirectUri = URL,
				GrantType = GrantTypes.AuthorizationCode,
				Parameters = 
				{
					{ OidcConstants.AuthorizeRequest.ResponseType, OidcConstants.ResponseTypes.Code },
					{ OidcConstants.AuthorizeRequest.CodeChallenge, KeyGenerator.GetUniqueKey(128) },
					{ OidcConstants.AuthorizeRequest.CodeChallengeMethod, OidcConstants.CodeChallengeMethods.Sha256 },
					{ OidcConstants.AuthorizeRequest.Scope, "api1" },
				}
			});
			if(response.HttpStatusCode != HttpStatusCode.Redirect)
			{
				throw new Exception(response.Error);
			}
			
			return response.HttpResponse.Headers.GetValues("Location").First();

			/*var response = await m_authClient.RequestTokenAsync(new TokenRequest()
			{
				RequestUri = new Uri(m_authClient.BaseAddress, "connect/authorize"),
				ClientId = "spa",
				GrantType = "authorization_code",
				
			});

			/*var tokenResponse = await m_authClient.RequestPasswordTokenAsync(new PasswordTokenRequest
			{
				Address = disco.TokenEndpoint,
				ClientId = "spa",
				UserName = username,
				Password = password
			});
			if (tokenResponse.IsError)
			{
				throw new Exception(tokenResponse.Error);
			}
			
			var authorizeRequest = await m_authClient.RequestorizationCodeTokenAsync(new AuthorizationCodeTokenRequest()
			{
				RequestUri = new Uri(m_resourceClient.BaseAddress + url),
				ClientId = "spa",
				Code = tokenResponse.AccessToken,
			});
			if (authorizeRequest.IsError)
			{
				throw new Exception(authorizeRequest.Error);
			}

			m_resourceClient.SetBearerToken(authorizeRequest.AccessToken);*/
		}
	}
}
