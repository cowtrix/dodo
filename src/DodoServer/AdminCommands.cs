using Common;
using Common.Commands;
using Common.Extensions;
using Dodo.LocalGroups;
using Dodo.Rebellions;
using Dodo.Users;
using Dodo.Users.Tokens;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Resources
{
	public static class AdminCommands
	{
		const string APPROVE_CMD_REGEX = @"^approve";
		[Command(APPROVE_CMD_REGEX, "approve <type> <user>", "Approve a user to create a given resource.\n" + 
			"<type>: The type of resource to approve. Can be:\n" +
			"\t - " + nameof(Rebellion) + "\n" +
			"\t - " + nameof(LocalGroup) + "\n" +
			"<user>: A user ID, either their username or GUID.")]
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
			if(rm.Value == null)
			{
				throw new Exception($"Bad syntax: Resource type not found with name '{typeName}'. Valid types are 'rebellion' & 'localgroup'");
			}
			if(!typeof(IPublicResource).IsAssignableFrom(rm.Key))
			{
				throw new Exception($"Resource type not valid '{typeName}'");
			}
			var id = m.Groups[2].Value;
			var user = ResourceUtility.GetManager<User>().GetSingle(u => u.Guid.ToString() == id || u.Slug == id);
			if(user == null)
			{
				throw new Exception($"User not found with id '{id}'");
			}

			using var rsc = new ResourceLock(user);
			user = rsc.Value as User;
			var token = new Dodo.Users.Tokens.ResourceCreationToken(rm.Key);
			user.TokenCollection.AddOrUpdate(user, token);

			return $"User {user.Slug} was successfully approved to create a new {rm.Key}";
		}


		const string SEARCHUSER_CMD_REGEX = @"^searchuser";
		[Command(SEARCHUSER_CMD_REGEX, "searchuser <user>", "Get information about a user or email address.")]
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
			var user = ResourceUtility.GetManager<User>().GetSingle(u => u.Guid.ToString() == id || u.Slug == id);
			if (user == null)
			{
				throw new Exception($"User not found with id '{id}'");
			}
			return JsonConvert.SerializeObject(
				user.GenerateJsonView(EPermissionLevel.ADMIN, null, default),
				Formatting.Indented);
		}

		const string DELETEUSER_CMD_REGEX = @"^deleteresource";
		[Command(SEARCHUSER_CMD_REGEX, "deleteresource <id>", "Delete a resource.\n\t<id>: A resource ID, either the slug or GUID.")]
		public static string DeleteObject(CommandArguments cmd)
		{
			const string DELETEUSER_REGEX = "^deleteresource\\s+(.+)";
			var s = cmd.Raw;
			var m = Regex.Match(s, DELETEUSER_REGEX);
			if (!m.Success)
			{
				throw new Exception($"Bad syntax: Expecting `deleteresource <id>`");
			}
			var id = m.Groups[1].Value;
			var rsc = ResourceUtility.GetManager<User>().GetSingle(u => u.Guid.ToString() == id || u.Slug == id);
			if (rsc == null)
			{
				throw new Exception($"Resource not found with id '{id}'");
			}
			ResourceUtility.GetManager(rsc.GetType()).Delete(rsc);
			return $"Resource {rsc} was successfully deleted.";
		}
	}

}
