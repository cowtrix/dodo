using Dodo.Users;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading.Tasks;

namespace Dodo.Email
{
	/// <summary>
	/// This class will manage sending out emails to users about various updates
	/// </summary>
	public static class UserEmailManager
	{
		private struct Update
		{
			public DateTime Timestamp;
			public string Text;
		}

		static UserEmailManager()
		{
			var updateTask = new Task(async () => await ProcessUpdateQueue());
			updateTask.Start();
		}

		private static IResourceManager<User> UserManager = ResourceUtility.GetManager<User>();
		private static Dictionary<Guid, List<Update>> m_pendingUpdates = new Dictionary<Guid, List<Update>>();

		private static async Task ProcessUpdateQueue()
		{
			while(true)
			{
				await Task.Delay(TimeSpan.FromMinutes(1));
				// Clear the queue for work
				Dictionary<Guid, List<Update>> updates;
				lock (m_pendingUpdates)
				{
					updates = m_pendingUpdates;
					m_pendingUpdates = new Dictionary<Guid, List<Update>>();
				}
				var userList = UserManager.Get(u => true);
				foreach(var update in updates)
				{
					var rsc = ResourceUtility.GetResourceByGuid(update.Key, force: true) as IGroupResource;
					foreach (var user in userList.Where(u => rsc.IsMember(u))) ;
				}
			}
		}

		public static void RegisterUpdate(IGroupResource rsc, string updateText)
		{
			var update = new Update
			{
				Text = updateText,
				Timestamp = DateTime.UtcNow,
			};
			lock (m_pendingUpdates)
			{
				if (!m_pendingUpdates.TryGetValue(rsc.Guid, out var list))
				{
					list = new List<Update>();
					m_pendingUpdates.Add(rsc.Guid, list);
				}
				list.Add(update);
			}
		}
	}
}
