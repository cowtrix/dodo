using Newtonsoft.Json;
using System.Linq;

namespace Dodo
{
	/// <summary>
	/// This is the network configuration for the server. 
	/// We would normally read this in via the .json configuration file (see: ConfigurationManager)
	/// </summary>
	public struct NetworkConfig
	{
		[JsonIgnore]
#if DEBUG
		public string FullURI => $"https://{Domains.First()}:{SSLPort}";
#else
		public string FullURI => $"https://{Domains.First()}";
#endif
		public string GetHostname()
		{
			var d = Domains.FirstOrDefault();
			if (d.StartsWith("www."))
			{
				return d.Substring(4);
			}
#if DEBUG
			if(d == "localhost")
			{
				d = $"{Dodo.DodoApp.PRODUCT_NAME}.com";
			}
#endif
			return d;
		}
		public int SSLPort;
		public string[] Domains;
		public bool LetsEncryptAutoSetup;
		public int HTTPPort;

		public NetworkConfig(int sslPort, int httpPort, bool letsEncrypt, params string[] domains)
		{
			Domains = domains;
			SSLPort = sslPort;
			HTTPPort = httpPort;
			LetsEncryptAutoSetup = letsEncrypt;
		}
	}
}
