using Newtonsoft.Json;
using System.Linq;

namespace Dodo
{
	public struct NetworkConfig
	{
		[JsonIgnore]
#if DEBUG
		public string FullURI => $"https://{Domain}:{SSLPort}";
#else
		public string FullURI => $"https://{Domain}";
#endif

		public string Hostname;
		public int SSLPort;
		public string Domain;
		public bool LetsEncryptAutoSetup;
		public int HTTPPort;

		public NetworkConfig(string domain, string hostname, int sslPort, int httpPort, bool letsEncrypt = false)
		{
			Hostname = hostname;
			Domain = domain;
			SSLPort = sslPort;
			HTTPPort = httpPort;
			LetsEncryptAutoSetup = letsEncrypt;
		}
	}
}
