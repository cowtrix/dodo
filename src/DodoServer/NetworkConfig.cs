using Newtonsoft.Json;
using System.Linq;

namespace DodoServer
{
	public struct NetworkConfig
	{
		[JsonIgnore]
		public string FullURI => $"https://{Domain}";

		public string IP;
		public int SSLPort;
		public string Domain;
		public bool LetsEncryptAutoSetup;
		internal object HTTPPort;

		public NetworkConfig(string domain, string ip, int sslPort, int httpPort, bool letsEncrypt = false)
		{
			IP = ip;
			Domain = domain;
			SSLPort = sslPort;
			HTTPPort = httpPort;
			LetsEncryptAutoSetup = letsEncrypt;
		}
	}
}
