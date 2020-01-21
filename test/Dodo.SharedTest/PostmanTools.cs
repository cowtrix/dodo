using Common.Config;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodoTest.Framework.Postman
{
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

		public void UpdateExampleJSON(string response, string category, string requestName, string exampleName = null)
		{
			if (m_postmanAPIKey.Value == "")
				return;
			var items = m_collection.Value<JObject>("collection").Value<JArray>("item");
			var cat = items.First(x => x.Value<string>("name") == category).Value<JArray>("item");
			var req = cat.First(x => x.Value<string>("name") == requestName).Value<JArray>("response").First;
			req["body"] = response;
		}

		public void Update()
		{
			if (m_postmanAPIKey.Value == "")
				return;
			var req = new RestRequest($"collections/{m_guid}", Method.PUT);
			req.AddParameter("text/json", m_collection.ToString(), ParameterType.RequestBody);
			var response = m_restClient.Execute(req);
		}
	}
}
