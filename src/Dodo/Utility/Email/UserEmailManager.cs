using Common;
using Common.Extensions;
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
				(string.IsNullOrEmpty(update.Header) ? "" : $"<h4>{update.Header}</h4>") +
				$"<p>{StringExtensions.TextToHtml(update.Text)}</p>" +
				"</td>\n" +
				"</tr>");
			return sb.ToString();
		}

		private struct Update
		{
			public DateTime Timestamp;
			public string Text;
			internal string Header;
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
				await Task.Delay(TimeSpan.FromSeconds(10));
				// Clear the queue for work
				Dictionary<Guid, List<Update>> updates;
				lock (m_pendingUpdates)
				{
					updates = m_pendingUpdates;
					m_pendingUpdates = new Dictionary<Guid, List<Update>>();
				}
				if(!updates.Any())
				{
					continue;
				}
				var userList = UserManager.Get(u => true);
				foreach(var update in updates)
				{
					var updateList = update.Value
						.OrderBy(u => u.Timestamp)
						.Select(UpdateToString);
					var updateText = string.Join('\n', updateList);
					var rsc = ResourceUtility.GetResourceByGuid(update.Key, ensureLatest: true) as IGroupResource;
					if(rsc == null)
					{
						continue;
					}
					Logger.Debug($"Sending out update for {rsc} @ {DateTime.UtcNow}: {updateText}");
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

		public static void RegisterUpdate(IPublicResource rsc, string header, string updateText)
		{
			if(!rsc.IsPublished) // Ignore as all drafting
			{
				return;
			}
			var update = new Update
			{
				Text = updateText,
				Header = header,
				Timestamp = DateTime.UtcNow,
			};
			Logger.Debug($"Registered update for {rsc} @ {DateTime.UtcNow}: {updateText}");
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
