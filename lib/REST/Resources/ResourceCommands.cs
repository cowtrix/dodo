using Common;
using Common.Commands;
using Common.Extensions;
using Newtonsoft.Json;
using System;
using System.Text.RegularExpressions;

namespace REST
{
	public static class ResourceCommands
	{
		const string SEARCH_CMD_REGEX = @"^resource\ssearch\s(.*)";
		[Command(SEARCH_CMD_REGEX, "resource search", "Search for a resource")]
		public static void SearchCommand(CommandArguments args)
		{
			throw new NotImplementedException();
			/*var rgx = Regex.Match(cmd, SEARCH_CMD_REGEX);
			var query = rgx.Groups[1].Value;
			Logger.Debug(JsonConvert.SerializeObject(ResourceUtility.Search<IRESTResource>(cmd), JsonExtensions.DatabaseSettings));*/
		}
	}

}
