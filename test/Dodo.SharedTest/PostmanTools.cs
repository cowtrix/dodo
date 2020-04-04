using Common.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Text;
using System.Threading.Tasks;

namespace DodoTest.Framework.Postman
{
	public struct PostmanEntryAddress
	{
		public string Category;
		public string Request;
	}

	public class PostmanCollection
	{
		ConfigVariable<string> m_postmanAPIKey = new ConfigVariable<string>("PostmanAPIKey", "");
		RestClient m_restClient = new RestClient("https://api.getpostman.com");
		JObject m_collection;
		string m_guid;

		public PostmanCollection(string guid)
		{
			m_guid = guid;
			m_restClient.AddDefaultHeader("X-Api-Key", m_postmanAPIKey.Value);
			var req = new RestRequest($"collections/{m_guid}", Method.GET);
			var response = m_restClient.Execute(req);
			m_collection = JsonConvert.DeserializeObject<JObject>(response.Content);
		}

		public void Update(PostmanEntryAddress entry, HttpResponseMessage req, int exampleIndex = 0)
		{
			if (string.IsNullOrEmpty(m_postmanAPIKey.Value))
				return;
			var items = m_collection.Value<JObject>("collection").Value<JArray>("item");
			var cat = items.First(x => x.Value<string>("name") == entry.Category).Value<JArray>("item");
			var item = cat.First(x => x.Value<string>("name") == entry.Request);

			var requestBody = Task.Run(async () => await req.RequestMessage.Content.ReadAsStringAsync().ConfigureAwait(false)).Result;
			var responseBody = Task.Run(async () => await req.Content.ReadAsStringAsync().ConfigureAwait(false)).Result;

			var request = item["request"];
			
			var response = item["response"][exampleIndex];
			response["originalRequest"]["method"] = req.RequestMessage.Method.Method;
			response["originalRequest"]["body"]["raw"] = requestBody;
			response["status"] = req.ReasonPhrase;
			response["code"] = (int)req.StatusCode;
			response["body"] = responseBody;
			item["response"][exampleIndex] = response;
		}

		public void Update()
		{
			if (string.IsNullOrEmpty(m_postmanAPIKey.Value))
				return;
			var req = new RestRequest($"collections/{m_guid}", Method.PUT);
			req.AddParameter("text/json", m_collection.ToString(), ParameterType.RequestBody);
			var response = m_restClient.Execute(req);
		}
	}
}
