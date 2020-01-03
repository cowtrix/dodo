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
		public CommandAttribute(string regex, string description)
		{
			CommandRegex = regex;
			Description = description;
		}
	}

	public static class CommandManager
	{
		private static Dictionary<Regex, MethodInfo> m_commands = new Dictionary<Regex, MethodInfo>();
		static CommandManager()
		{
			var allMethods = AppDomain.CurrentDomain.GetAssemblies().Select(assembly => assembly.GetTypes()).ConcatenateCollection()
				.Select(x => x.GetMethods().Where(method => method.GetCustomAttribute<CommandAttribute>() != null)).ConcatenateCollection();
			foreach(var method in allMethods)
			{
				if(!method.IsStatic)
				{
					throw new Exception("Cannot hook command for non-static function");
				}
				var par = method.GetParameters();
				if (par.Length != 1 || par.Single().ParameterType != typeof(string))
				{
					throw new Exception($"Method {method.DeclaringType.Name}:{method.Name} parameter must be a single string variable");
				}
				var attr = method.GetCustomAttribute<CommandAttribute>();
				m_commands.Add(new Regex(attr.CommandRegex), method);
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
			cmd.Value.Invoke(null, new[] { line });
		}
	}
}
