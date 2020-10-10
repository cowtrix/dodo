using Common;
using Common.Commands;
using Common.Extensions;
using Dodo.Users;
using Newtonsoft.Json;
using System;
using System.Linq;
using System.Text.RegularExpressions;

namespace Resources
{
	public static class AdminCommands
	{
		const string SEARCH_CMD_REGEX = @"^approve";
		const string SEARCH_REGEX = "^approve\\s+(.+?)\\s+(.+?)$";
		[Command(SEARCH_CMD_REGEX, "approve <type> <user>", "Approve a user to create a given resource")]
		public static string SearchCommand(CommandArguments cmd)
		{
			var s = cmd.Raw;
			var m = Regex.Match(s, SEARCH_REGEX);
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
				throw new Exception($"Error: Resource type not valid '{typeName}'");
			}
			var id = m.Groups[2].Value;
			var user = ResourceUtility.GetManager<User>().GetSingle(u => u.Guid.ToString() == id || u.Slug == id);
			if(user == null)
			{
				throw new Exception($"Error: User not found with id '{id}'");
			}

			using var rsc = new ResourceLock(user);
			user = rsc.Value as User;
			var token = new Dodo.Users.Tokens.ResourceCreationToken(rm.Key);
			user.TokenCollection.AddOrUpdate(user, token);

			return "Success";
		}
	}

}
