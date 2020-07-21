using Common;
using Common.Extensions;
using Common.Security;
using Resources;
using System;

namespace Dodo.Utility
{
	/// <summary>
	/// This is a simple email anonimization service that routes conversations between
	/// different user accounts, and masks the addresses.
	/// </summary>
	public static class EmailProxy
	{
		const int SaltSize = 16;

		public class ProxyInformation : IVerifiable
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
			[Email]
			public string RemoteEmail { get; set; }
			/// <summary>
			/// This is the email that the RemoteEmail should respond to, which
			/// should be a local account
			/// </summary>
			[Email]
			public string ProxyEmail { get; set; }

			public bool CanVerify() => true;
			public string GetKey() => SHA256Utility.SHA256(RemoteEmail + ProxyEmail);

			public bool VerifyExplicit(out string error)
			{
				error = null;
				return true;
			}
		}

		private static PersistentStore<string, ProxyInformation> m_proxy 
			= new PersistentStore<string, ProxyInformation>(Dodo.DodoApp.PRODUCT_NAME, "mailproxy");
		public static ProxyInformation GetProxyFromKey(string fromEmail, string proxyEmail)
		{
			if(!m_proxy.TryGetValue(SHA256Utility.SHA256(fromEmail + proxyEmail), out var proxy))
			{
				Logger.Warning($"Unable to resolve proxy for {fromEmail} + {proxyEmail}");
			}
			return proxy;
		}

		public static ProxyInformation SetProxy(string fromEmail, string toEmail)
		{
			var incomingProxy = GenerateNewProxy(fromEmail);
			var outgoingProxy = GenerateNewProxy(toEmail);
			m_proxy[incomingProxy.GetKey()] = outgoingProxy;
			Logger.Debug($"Set proxy {incomingProxy.GetKey().Substring(0, 8)}... {outgoingProxy.ProxyEmail} -> {outgoingProxy.RemoteEmail}");
			m_proxy[outgoingProxy.GetKey()] = incomingProxy;
			Logger.Debug($"Set proxy {outgoingProxy.GetKey().Substring(0, 8)}... {incomingProxy.ProxyEmail} -> {incomingProxy.RemoteEmail}");
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
			if(!proxy.Verify(out var error))
			{
				throw new Exception(error);
			}
			return proxy;
		}
	}
}
