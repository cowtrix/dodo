using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;

namespace Common
{
	[AttributeUsage(AttributeTargets.Method)]
	public class CommandAttribute : Attribute
	{
		public readonly string CommandRegex;
		public readonly string Description;
		public readonly string FriendlyName;
		public CommandAttribute(string regex, string friendlyname, string description)
		{
			CommandRegex = regex;
			Description = description;
			FriendlyName = friendlyname;
		}
	}

	public static class CommandManager
	{
		private struct CommandData
		{
			public MethodInfo Method;
			public CommandAttribute Attribute;
		}
		private static Dictionary<Regex, CommandData> m_commands = new Dictionary<Regex, CommandData>();
		static CommandManager()
		{
			LoadCommands();
		}

		private static void LoadCommands()
		{
			var allMethods = AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetTypes()).ConcatenateCollection()
				.Select(x => x.GetMethods().Where(method => method.GetCustomAttribute<CommandAttribute>() != null)).ConcatenateCollection();
			foreach (var method in allMethods)
			{
				if (!method.IsStatic)
				{
					throw new Exception("Cannot hook command for non-static function");
				}
				var par = method.GetParameters();
				if (par.Length != 1 || par.Single().ParameterType != typeof(string))
				{
					throw new Exception($"Method {method.DeclaringType.Name}:{method.Name} parameter must be a single string variable");
				}
				var attr = method.GetCustomAttribute<CommandAttribute>();
				m_commands.Add(new Regex(attr.CommandRegex), new CommandData
				{
					Attribute = attr,
					Method = method,
				});
			}
		}

		internal static void Execute(string line)
		{
			var cmds = m_commands.Where(x => x.Key.IsMatch(line));
			var cmdCount = cmds.Count();
			if(cmdCount > 1)
			{
				throw new Exception("Ambigious command");
			}
			if(cmdCount == 0)
			{
				throw new Exception("Command not found");
			}
			var cmd = cmds.Single();
			cmd.Value.Method.Invoke(null, new[] { line });
		}

		[Command(@"^help$", "help", "List all commands")]
		public static void Help(string args)
		{
			foreach(var cmd in m_commands.OrderBy(x => x.Value.Attribute.FriendlyName))
			{
				Console.WriteLine($"  {cmd.Value.Attribute.FriendlyName,-20}\t\t{cmd.Value.Attribute.Description, 10}");
			}
		}
	}
}
