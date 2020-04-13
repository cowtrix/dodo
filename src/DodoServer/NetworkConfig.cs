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

		public NetworkConfig(string domain, string ip, int sslPort, bool letsEncrypt = false)
		{
			IP = ip;
			Domain = domain;
			SSLPort = sslPort;
			LetsEncryptAutoSetup = letsEncrypt;
		}
	}
}
