using Newtonsoft.Json;
using System.Linq;

namespace Dodo
{
	public struct NetworkConfig
	{
		[JsonIgnore]
#if DEBUG
		public string FullURI => $"https://{Domains.First()}:{SSLPort}";
#else
		public string FullURI => $"https://{Domains.First()}";
#endif

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
