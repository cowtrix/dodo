using Common;
using Common.Config;
using Common.Extensions;
using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.IO;
using System.Linq;
using System.Net.Http;
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
			File.WriteAllText("postmancollection_before.json", m_collection.ToString());
		}

		public void Update(PostmanEntryAddress entry, HttpResponseMessage req, int exampleIndex = 0, string exampleName = null)
		{
			if (string.IsNullOrEmpty(m_postmanAPIKey.Value))
				return;
			var items = m_collection.Value<JObject>("collection").Value<JArray>("item");
			var cat = items.First(x => x.Value<string>("name") == entry.Category).Value<JArray>("item");
			var item = cat.First(x => x.Value<string>("name").Replace(" ", "") == entry.Request.Replace(" ", ""));

			var requestBody = req.RequestMessage.Content == null ? "" :
				JsonExtensions.PrettifyJSON(Task.Run(async () => await req.RequestMessage.Content.ReadAsStringAsync().ConfigureAwait(false))?.Result);
			var responseBody = req.Content == null ? "" :
				JsonExtensions.PrettifyJSON(Task.Run(async () => await req.Content.ReadAsStringAsync().ConfigureAwait(false))?.Result);

			var request = item["request"];
			if(!string.IsNullOrEmpty(exampleName))
			{
				request["name"] = exampleName;
			}
			const string autogen = "Auto generated\n\n";
			var desc = request.Value<string>("description");
			if (string.IsNullOrEmpty(desc))
			{
				desc = autogen;
			}
			else if (!desc.StartsWith(autogen))
			{
				desc = autogen + desc;
			}
			item["request"]["description"] = desc;

			var response = item["response"][exampleIndex];
			response["originalRequest"]["method"] = req.RequestMessage.Method.Method;
			response["originalRequest"]["url"]["raw"] = req.RequestMessage.RequestUri.OriginalString;
			response["originalRequest"]["url"]["host"] = new JArray(new [] { $"https://{req.RequestMessage.RequestUri.Host}:{req.RequestMessage.RequestUri.Port}" });
			response["originalRequest"]["url"]["path"] = new JArray(req.RequestMessage.RequestUri.AbsolutePath.Split(new[] { "/" }, StringSplitOptions.RemoveEmptyEntries));
			if (response["originalRequest"].Value<JObject>("body") != null)
			{
				response["originalRequest"]["body"]["raw"] = requestBody;
			}
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
			File.WriteAllText("postmancollection_after.json", m_collection.ToString());
			req.AddParameter("text/json", m_collection.ToString(), ParameterType.RequestBody);
			var response = m_restClient.Execute(req);
			if(!response.IsSuccessful)
			{
				throw new Exception($"Failed to update Postman documentation: {response}");
			}
			Logger.Info("Updated Postman documentation");
		}
	}
}
