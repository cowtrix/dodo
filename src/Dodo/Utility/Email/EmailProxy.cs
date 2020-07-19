using Common;
using Common.Security;
using Resources;

namespace Dodo.Utility
{
	public static class EmailProxy
	{
		const int SaltSize = 16;

		public class ProxyInformation
		{
			public ProxyInformation(string email, string proxy)
			{
				RemoteEmail = email;
				ProxyEmail = proxy;
			}
			/// <summary>
			/// This is the email that messages will be forwarded to if they
			/// get the key
			/// </summary>
			public string RemoteEmail { get; set; }
			/// <summary>
			/// This is the email that the RemoteEmail should respond to, which
			/// should be a local account
			/// </summary>
			public string ProxyEmail { get; set; }
			public string GetKey() => SHA256Utility.SHA256(RemoteEmail + ProxyEmail);
		}

		private static PersistentStore<string, ProxyInformation> m_proxy 
			= new PersistentStore<string, ProxyInformation>(Dodo.DodoApp.PRODUCT_NAME, "mailproxy");
		public static ProxyInformation GetProxyFromKey(string fromEmail, string proxyEmail)
		{
			if(!m_proxy.TryGetValue(SHA256Utility.SHA256(fromEmail + proxyEmail), out var proxy))
			{
				if(m_proxy.TryGetValue(SHA256Utility.SHA256(proxyEmail + fromEmail), out proxy))
				{
					Logger.Warning($"Had to reverse proxy for some reason?");
				}
			}
			return proxy;
		}

		public static ProxyInformation SetProxy(string fromEmail, string toEmail)
		{
			var incomingProxy = GenerateNewProxy(fromEmail);
			var outgoingProxy = GenerateNewProxy(toEmail);
			m_proxy[incomingProxy.GetKey()] = outgoingProxy;
			m_proxy[outgoingProxy.GetKey()] = incomingProxy;
			return outgoingProxy;
		}

		private static ProxyInformation GenerateNewProxy(string email)
		{
			ProxyInformation proxy;
			do
			{
				var proxyEmail = $"{KeyGenerator.GetUniqueKey(SaltSize).ToLowerInvariant()}@{Dodo.DodoApp.NetConfig.GetHostname()}";
				proxy = new ProxyInformation(email, proxyEmail);
			} 
			while (m_proxy.ContainsKey(proxy.GetKey()));
			return proxy;
		}
	}
}
