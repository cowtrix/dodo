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
		private static DodoRESTServer m_restServer;
		private static BackupManager m_backupManager;
		private static Dictionary<Type, IResourceManager> m_resourceManagers;

		static void Main(string[] args)
		{
			Initialise();
		}

		public static void Initialise()
		{
			m_resourceManagers = new Dictionary<Type, IResourceManager>
			{
				{ typeof(User), new UserManager() },
				{ typeof(Rebellion), new RebellionManager() },
			};
			/*m_backupManager = new BackupManager();
			foreach(var rm in m_resourceManagers)
			{
				m_backupManager.RegisterBackup(rm.Value);
			}*/
			m_restServer = new DodoRESTServer(8080);
			m_restServer.Start();
		}

		public static IResourceManager<T> ResourceManager<T>() where T: Resource
		{
			return m_resourceManagers[typeof(T)] as IResourceManager<T>;
		}

		public static void CleanAllData()
		{
			foreach (var rm in m_resourceManagers)
			{
				rm.Value.Clear();
			}
		}
	}
}
