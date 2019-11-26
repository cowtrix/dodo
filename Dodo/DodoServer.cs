using Common;
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
using System.Threading.Tasks;

namespace Dodo
{
	public static class DodoServer
	{
		private static ConfigVariable<int> m_httpPort = new ConfigVariable<int>("HttpPort", 443);
		private static ConfigVariable<string> m_sslCertPath = new ConfigVariable<string>("SSLCertificatePath", "server.pfx");

		private static DodoRESTServer m_restServer;
		private static BackupManager m_backupManager;
		private static Dictionary<Type, IResourceManager> m_resourceManagers;

		static void Main(string[] args)
		{
			Initialise();
		}

		public static void Initialise()
		{
			// TODO automatically gather these?
			m_resourceManagers = new Dictionary<Type, IResourceManager>
			{
				{ typeof(User), new UserManager() },
				{ typeof(Rebellion), new RebellionManager() },
				{ typeof(LocalGroup), new LocalGroupManager() },
				{ typeof(WorkingGroup), new WorkingGroupManager() },
				{ typeof(Task), new WorkingGroupManager() },
				{ typeof(Role), new RoleManager() },
			};
			/*m_backupManager = new BackupManager();
			foreach(var rm in m_resourceManagers)
			{
				m_backupManager.RegisterBackup(rm.Value);
			}*/
			m_restServer = new DodoRESTServer(m_httpPort.Value, m_sslCertPath.Value, "password");
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
