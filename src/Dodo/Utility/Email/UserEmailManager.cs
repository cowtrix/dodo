using Dodo.Users;
using Resources;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Dodo.Email
{
	/// <summary>
	/// This class will manage sending out emails to users about various updates
	/// </summary>
	public static class UserEmailManager
	{
		private static string UpdateToString(Update update)
		{
			const string trStyle =
				"color: #153643;" + 
				"font-family: 'Trebuchet MS', Helvetica, sans-serif;" + 
				"font-size: 16px;" + 
				"line-height: 24px;" + 
				"padding: 20px 0 30px 0;";
			var sb = new StringBuilder();
			sb.AppendLine($"<tr style=\"{trStyle}\">\n" +
				"<td>\n" +
				$"<small>{update.Timestamp.ToLongTimeString()}</small>\n" +
				$"<p>{update.Text}</p>\n" +
				"</td>\n" +
				"</tr>");
			return sb.ToString();
		}

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
				await Task.Delay(TimeSpan.FromMinutes(30));
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
					var updateList = update.Value
						.Select(UpdateToString);
					var updateText = string.Join('\n', updateList);
					var rsc = ResourceUtility.GetResourceByGuid(update.Key, force: true) as IGroupResource;
					if(rsc == null)
					{
						continue;
					}
					foreach (var user in userList
						.Where(u => u.PersonalData.EmailPreferences.NewNotifications && rsc.IsMember(u)))
					{
						EmailUtility.SendEmail(new EmailAddress(user.PersonalData.Email, user.Slug),
							$"[{DodoApp.PRODUCT_NAME}] Update from {rsc.Name}",
							"Update", new Dictionary<string, string> 
							{ 
								{ "UPDATES", updateText },
								{ "RESOURCE_LINK", $"{DodoApp.NetConfig.FullURI}/{rsc.GetType().Name.ToLowerInvariant()}/{rsc.Slug}"}
							});
					}
				}
			}
		}

		public static void RegisterUpdate(IPublicResource rsc, string updateText)
		{
			if(!rsc.IsPublished)
			{
				return;
			}
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
