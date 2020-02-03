using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text.RegularExpressions;

namespace Common.Commands
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
			public Action<CommandArguments> Method;
			public CommandAttribute Attribute;
		}
		private static Dictionary<Regex, CommandData> m_commands = new Dictionary<Regex, CommandData>();
		private static CommandData m_fallbackCommand;   // The command to run when all else fails - default is help
		public static Action HelpOverride;
		public static Action<CommandArguments> OnPreExecute;

		private static List<string> m_ignoredAssemblies = new List<string>()
		{
			"Microsoft.", "System."
		};


		static CommandManager()
		{
			LoadCommands();
		}

		private static void LoadCommands()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(ass => !m_ignoredAssemblies.Any(ign => ass.FullName.StartsWith(ign)));
			var allTypes = assemblies.Select(assembly => assembly.GetTypes()).ConcatenateCollection();
			var allMethods = allTypes.Select(x => x.GetMethods().Where(method => method.GetCustomAttribute<CommandAttribute>() != null)).ConcatenateCollection();
			foreach (var method in allMethods)
			{
				if (!method.IsStatic)
				{
					throw new Exception("Cannot hook command for non-static function");
				}
				var par = method.GetParameters();
				if (par.Length != 1 || par.Single().ParameterType != typeof(CommandArguments))
				{
					throw new Exception($"Method {method.DeclaringType.Name}:{method.Name} parameter must be a single CommandArguments variable");
				}
				var attr = method.GetCustomAttribute<CommandAttribute>();
				var cmdData = new CommandData
				{
					Attribute = attr,
					Method = args => method.Invoke(null, new object[] { args }),
				};
				if (attr.CommandRegex == null)
				{
					m_fallbackCommand = cmdData;
				}
				else
				{
					m_commands.Add(new Regex(attr.CommandRegex), cmdData);
				}
			}
			if (m_fallbackCommand.Method == null)
			{
				m_fallbackCommand = new CommandData() { Method = args => Help(args) };
			}
		}

		public static void Execute(string line, CommandArgumentParameters parameters)
		{
			try
			{
				var cmds = m_commands.Where(x => x.Key.IsMatch(line));
				var cmdCount = cmds.Count();
				if (cmdCount > 1)
				{
					throw new Exception("Ambigious command");
				}
				CommandData cmd;
				if (cmdCount == 0)
				{
					cmd = m_fallbackCommand;
				}
				else
				{
					cmd = cmds.Single().Value;
				}
				var args = new CommandArguments(line, parameters);
				OnPreExecute?.Invoke(args);
				cmd.Method.Invoke(args);
			}
			catch (Exception e)
			{
				Logger.Exception(e);
			}
		}

		public static void Execute(string[] args, CommandArgumentParameters parameters)
		{
			Execute(string.Join(" ", args), parameters);
		}

		public static void Execute(string[] args)
		{
			Execute(args, CommandArgumentParameters.Default);
		}

		public static void Execute(string line)
		{
			Execute(line, CommandArgumentParameters.Default);
		}

		[Command(@"^help$", "help", "List all commands")]
		public static void Help(CommandArguments args)
		{
			if (HelpOverride != null)
			{
				HelpOverride.Invoke();
				return;
			}
			foreach (var cmd in m_commands.OrderBy(x => x.Value.Attribute.FriendlyName))
			{
				Console.WriteLine($"  {cmd.Value.Attribute.FriendlyName,-20}\t\t{cmd.Value.Attribute.Description,10}");
			}
		}
	}
}
