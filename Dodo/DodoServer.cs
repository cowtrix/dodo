﻿using Common;
using Common.Config;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Roles;
using Dodo.Users;
using Dodo.WorkingGroups;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace Dodo
{
	public static class DodoServer
	{
		public const string PRODUCT_NAME = "Dodo";
		private static ConfigVariable<int> m_httpPort = new ConfigVariable<int>("HttpsPort", 443);
		private static ConfigVariable<string> m_sslCertPath = new ConfigVariable<string>("SSLCertificatePath", "server.pfx");

		private static DodoRESTServer m_restServer;
		private static CommandReader m_commandReader = new CommandReader();

		static void Main(string[] args)
		{
			Initialise();
			Thread.Sleep(-1);
		}

		public static void Initialise(string[] args = null)
		{
			m_restServer = new DodoRESTServer(m_httpPort.Value, m_sslCertPath.Value, "password");
			m_restServer.Start();
		}

		public static void CleanAllData()
		{
			ResourceUtility.Clear();
		}

		public static string GetURL()
		{
			return m_restServer.GetURL();
		}
	}
}
