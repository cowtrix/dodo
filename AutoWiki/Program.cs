using Newtonsoft.Json;
using Newtonsoft.Json.Linq;
using RestSharp;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AutoWiki
{
	public class Configuration
	{
		public string RepositoryURL = "";
		public string ProjectName = "";
		public string APIToken = "";
		public string Output = "";
		public string[] Assemblies = new string[0];
	}

	static class Program
	{
		static Configuration m_config;
		static RestClient m_restClient;

		static void Main(string[] args)
		{
			var path = Path.GetFullPath(args[0]);
			if(!File.Exists(path))
			{
				File.WriteAllText(path, JsonConvert.SerializeObject(new Configuration(), Formatting.Indented));
				Console.WriteLine($"Wrote sample config to {path}. Fill this out and try again.");
				return;
			}
			m_config = JsonConvert.DeserializeObject<Configuration>(File.ReadAllText(args[0]));
			m_restClient = new RestClient(m_config.RepositoryURL);
			m_restClient.AddDefaultHeader("Private-Token", m_config.APIToken);

			var pgs = GetAllPages();
			foreach(var pg in pgs)
			{
				DeletePage(pg.Value<string>("slug"));
			}

			// Build the markdown
			Console.WriteLine(ExecuteConsoleCommand("dotnet tool install -g loxsmoke.mddox", true));
			var fullOutput = Path.GetFullPath(m_config.Output);
			if(!Directory.Exists(fullOutput))
			{
				Directory.CreateDirectory(fullOutput);
			}
			foreach(var assembly in m_config.Assemblies)
			{
				var fullAssembly = Path.GetFullPath(assembly);
				if(!File.Exists(fullAssembly))
				{
					throw new FileNotFoundException(fullAssembly);
				}
				var friendlyName = Path.GetFileNameWithoutExtension(fullAssembly);
				var output = $"{fullOutput}\\{friendlyName}.md";
				Console.WriteLine(ExecuteConsoleCommand("mddox \"{0}\" --output \"{1}\"", true, fullAssembly, output));
				CreateWikiPage("API: " + friendlyName, File.ReadAllText(output));
			}
		}

		private static string ProjectURI
		{
			get
			{
				return $"api/v4/projects/{Uri.EscapeDataString(m_config.ProjectName)}";
			}
		}

		private static List<JObject> GetAllPages()
		{
			var createRequest = new RestRequest($"{ProjectURI}/wikis", Method.GET);
			var response = m_restClient.Execute(createRequest);
			Console.WriteLine(response.ResponseStatus);
			return JsonConvert.DeserializeObject<JArray>(response.Content).Values<JObject>().ToList();
		}

		private static void DeletePage(string slug)
		{
			//
			var createRequest = new RestRequest($"{ProjectURI}/wikis/{slug}", Method.DELETE);
			var response = m_restClient.Execute(createRequest);
			Console.WriteLine(response.ResponseStatus);
		}

		private static void CreateWikiPage(string title, string content)
		{
			var createRequest = new RestRequest($"{ProjectURI}/wikis", Method.POST);
			createRequest.AddParameter("title", title);
			createRequest.AddParameter("content", content);
			var response = m_restClient.Execute(createRequest);
			Console.WriteLine(response.ResponseStatus);
			Console.WriteLine(response.Content);
		}

		public static string ExecuteConsoleCommand(string cmd, bool failOnStderr, params string[] args)
		{
			var cmdParams = new ProcessStartInfo()
			{
				FileName = "CMD.exe",
				RedirectStandardInput = true,
				RedirectStandardOutput = true,
				RedirectStandardError = true,
				UseShellExecute = false,
				Arguments = "/C"
			};
			var cmdProcess = Process.Start(cmdParams);
			while (cmdProcess.StandardOutput.Peek() > -1)
			{
				cmdProcess.StandardOutput.ReadLine();
			}
			cmdProcess.StandardInput.WriteLine(string.Format(cmd, args));
			cmdProcess.StandardInput.Flush();
			cmdProcess.StandardInput.Close();
			cmdProcess.WaitForExit();
			var err = cmdProcess.StandardError.ReadToEnd();
			if (!string.IsNullOrEmpty(err) && failOnStderr)
			{
				throw new Exception(err);
			}
			var str = cmdProcess.StandardOutput.ReadToEnd();
			return str;
		}
	}


}
