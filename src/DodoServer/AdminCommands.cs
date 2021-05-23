using Common;
using Common.Commands;
using Common.Extensions;
using Dodo.Email;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Users;
using Dodo.Users.Tokens;
using MongoDB.Driver;
using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Resources
{
	public static class AdminCommands
	{
		[Command(@"^approve", "approve <type> <user>", "Approve a user to create a given resource.\n" +
			"\t<type>: The type of resource to approve. Can be:\n" +
			"\t\t - " + nameof(Rebellion) + "\n" +
			"\t\t - " + nameof(LocalGroup) + "\n" +
			"\t<user>: A user ID, either their username or GUID.")]
		public static string ApproveForResource(CommandArguments cmd)
		{
			const string APPROVE_REGEX = "^approve\\s+(.+?)\\s+(.+?)$";
			var s = cmd.Raw;
			var m = Regex.Match(s, APPROVE_REGEX);
			if (!m.Success)
			{
				throw new Exception($"Bad syntax: Expecting `approve <type> <user>`");
			}
			var typeName = m.Groups[1].Value;
			var rm = ResourceUtility.ResourceManagers.SingleOrDefault(rm => rm.Key.Name.ToLowerInvariant() == typeName.ToLowerInvariant());
			if (rm.Value == null)
			{
				throw new Exception($"Bad syntax: Resource type not found with name '{typeName}'. Valid types are 'rebellion' & 'localgroup'");
			}
			if (!typeof(IPublicResource).IsAssignableFrom(rm.Key))
			{
				throw new Exception($"Resource type not valid '{typeName}'");
			}
			var id = m.Groups[2].Value;
			var userManager = ResourceUtility.GetManager<User>();
			var user = userManager.GetSingle(u => u.Guid.ToString() == id || u.Slug == id || u.PersonalData.Email == id);
			if (user == null)
			{
				throw new Exception($"User not found with id '{id}'");
			}

			using (var rscLock = new ResourceLock(user))
			{
				user = rscLock.Value as User;
				var token = new Dodo.Users.Tokens.ResourceCreationToken(rm.Key);
				user.TokenCollection.AddOrUpdate(user, token);
				userManager.Update(user, rscLock);
			}


			var friendlyTypeName = rm.Key.GetName();
			EmailUtility.SendEmail(new EmailAddress(user.PersonalData.Email, user.Name),
				$"[{Dodo.DodoApp.PRODUCT_NAME}] You've been approved to create a new {friendlyTypeName}",
				"Callback", new Dictionary<string, string>
				{
						{ "MESSAGE", $"You've been approved to create a new {friendlyTypeName}. To do so, follow the link below and follow the steps." },
						{ "CALLBACK_MESSAGE", $"Create A New {friendlyTypeName}" },
						{ "CALLBACK_URL", $"{Dodo.DodoApp.NetConfig.FullURI}/edit/{typeName.ToLowerInvariant()}/create" }
				});

			return $"User {user.Slug} was successfully approved to create a new {rm.Key}";
		}

		[Command(@"^searchuser", "searchuser <user>", "Get information about a user or email address.")]
		public static string SearchForUser(CommandArguments cmd)
		{
			const string SEARCHUSER_REGEX = "^searchuser\\s+(.+)";
			var s = cmd.Raw;
			var m = Regex.Match(s, SEARCHUSER_REGEX);
			if (!m.Success)
			{
				throw new Exception($"Bad syntax: Expecting `searchuser <user>`");
			}
			var id = m.Groups[1].Value;
			var user = ResourceUtility.GetManager<User>().GetSingle(u => u.Guid.ToString() == id || u.Slug == id || u.PersonalData.Email == id);
			if (user == null)
			{
				throw new Exception($"User not found with id '{id}'");
			}
			return JsonConvert.SerializeObject(
				user.GenerateJsonView(EPermissionLevel.ADMIN, null, default),
				Formatting.Indented);
		}

		[Command(@"^delete", "delete <id>",
			"Delete a resource.\n\t<id>: A resource ID, either the slug or GUID. Use with caution!")]
		public static string DeleteResource(CommandArguments cmd)
		{
			const string DELETEUSER_REGEX = "^deleteresource\\s+(.+)";
			var s = cmd.Raw;
			var m = Regex.Match(s, DELETEUSER_REGEX);
			if (!m.Success)
			{
				throw new Exception($"Bad syntax: Expecting `deleteresource <id>`");
			}
			var id = m.Groups[1].Value;
			var rsc = ResourceUtility.GetResource(u => u.Guid.ToString() == id || u.Slug == id)
				.FirstOrDefault();
			if (rsc == null)
			{
				throw new Exception($"Resource not found with id '{id}'");
			}
			ResourceUtility.GetManager(rsc.GetType()).Delete(rsc);
			return $"Resource {rsc} was successfully deleted.";
		}

		[Command(@"^reversedelete", "reversedelete <id>",
			"Reverse the deletion of a resource.\n\t<id>: A resource ID, either the slug or GUID.")]
		public static string ReverseDeleteResource(CommandArguments cmd)
		{
			const string REVERSE_DELETEUSER_REGEX = "^reversedelete\\s+(.+)";
			var s = cmd.Raw;
			var m = Regex.Match(s, REVERSE_DELETEUSER_REGEX);
			if (!m.Success)
			{
				throw new Exception($"Bad syntax: Expecting `reversedelete <id>`");
			}
			var id = m.Groups[1].Value;
			Guid.TryParse(id, out var guid);
			var filter = Builders<Resource>.Filter.Or(
				Builders<Resource>.Filter.Eq(x => x.Guid, guid),
				Builders<Resource>.Filter.Eq(x => x.Slug, id));
			var rsc = ResourceUtility.Trash.FindOneAndDelete(filter);
			if (rsc == null)
			{
				throw new Exception($"No resource found in trash with id '{id}'");
			}
			ResourceUtility.GetManager(rsc.GetType()).Add(rsc);
			ResourceUtility.Trash.DeleteOne(r => rsc.Guid == r.Guid);
			return $"Resource {rsc} was successfully reverted from deletion.";
		}

		[Command(@"^hide", "hide <id> <reason>",
			"Hide a resource from public view.\n\t<id>: A resource ID, either the slug or GUID.\n\t<reason>The reason for hiding, e.g. 'broke the rules doing x'.")]
		public static string HideResource(CommandArguments cmd)
		{
			const string HIDERESOURCE_REGEX = "^hide\\s+(.+?)\\s+(.+)";
			var s = cmd.Raw;
			var m = Regex.Match(s, HIDERESOURCE_REGEX);
			if (!m.Success)
			{
				throw new Exception($"Bad syntax: Expecting `hide <id> <reason>`");
			}
			var id = m.Groups[1].Value;
			var reason = m.Groups[2].Value;
			var rsc = ResourceUtility.GetResource(u => u.Guid.ToString() == id || u.Slug == id)
				.FirstOrDefault();
			if (rsc == null || !(rsc is IPublicResource pub))
			{
				throw new Exception($"Public resource not found with id '{id}'");
			}
			pub.Hide(reason);
			return $"Resource {rsc} was successfully hidden.";
		}

		[Command(@"^unhide", "unhide <id>",
			"Unhide a resource, which will make it publicly viewable again.\n\t<id>: A resource ID, either the slug or GUID.")]
		public static string UnhideResource(CommandArguments cmd)
		{
			const string UNHIDERESOURCE_REGEX = "^unhide\\s+(.+)";
			var s = cmd.Raw;
			var m = Regex.Match(s, UNHIDERESOURCE_REGEX);
			if (!m.Success)
			{
				throw new Exception($"Bad syntax: Expecting `unhide <id>`");
			}
			var id = m.Groups[1].Value;
			var rsc = ResourceUtility.GetResource(u => u.Guid.ToString() == id || u.Slug == id)
				.FirstOrDefault();
			if (rsc == null || !(rsc is IPublicResource pub))
			{
				throw new Exception($"Public resource not found with id '{id}'");
			}
			pub.UnHide();
			return $"Resource {rsc} was successfully unhidden.";
		}
	}

}
