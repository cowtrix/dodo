using Common;
using Dodo.Rebellions;
using Dodo.Users;
using SimpleHttpServer.REST;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodo
{
	public static class DodoServer
	{
		private static RESTServer m_restServer;
		public static UserManager UserManager;
		public static RebellionManager RebellionManager;
		private static BackupManager m_backupManager;

		static void Main(string[] args)
		{
			Initialise();
		}

		public static void Initialise()
		{
			UserManager = new UserManager();
			RebellionManager = new RebellionManager();
			m_backupManager = new BackupManager()
				.RegisterBackup(UserManager)
				.RegisterBackup(RebellionManager);
			m_restServer = new RESTServer(8080);
		}

		public static void CleanAllData()
		{
			UserManager.Clear();
		}
	}
}
