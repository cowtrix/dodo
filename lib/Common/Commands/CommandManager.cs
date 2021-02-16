using Common.Extensions;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Reflection;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;

namespace Common.Commands
{
	/// <summary>
	/// A Command attribute indicates that a method can be invoked if the regex pattern is matched
	/// by the commandline arguments given to CommandManager.Execute().
	/// The method must be public, static, and take a single parameter of type CommandArguments
	/// </summary>
	[AttributeUsage(AttributeTargets.Method)]
	public class CommandAttribute : Attribute
	{
		public readonly string CommandRegex;
		public readonly string Description;
		public readonly string FriendlyName;

		/// <summary>
		/// Mark this method as a Command
		/// </summary>
		/// <param name="regex">The regex to be matched</param>
		/// <param name="friendlyname">The name of the command to be displayed in the help</param>
		/// <param name="description">The description of the command to be displayed in the help</param>
		public CommandAttribute(string regex, string friendlyname, string description)
		{
			CommandRegex = regex;
			Description = description;
			FriendlyName = friendlyname;
		}
	}

	/// <summary>
	/// The Command Manager routes a commandline call to a method call,
	/// and converts the arguments into a easy to use handler.
	/// </summary>
	public static class CommandManager
	{
		/// <summary>
		/// Represents constructed metadata around each `CommandAttribute` usage
		/// </summary>
		public struct CommandData
		{
			public Func<CommandArguments, string> Method;
			public CommandAttribute Attribute;
		}
		private static Dictionary<Regex, CommandData> m_commands = new Dictionary<Regex, CommandData>();
		private static CommandData m_fallbackCommand;   // The command to run when no other command matches - default is help
		public static Func<string> HelpOverride;  // Help is a built-in thing but we can override it if we wish
		public static Action<CommandArguments> OnPreExecute;    // Sometimes we want to jump in before the command is fired

		public static IEnumerable<CommandData> AllCommands => m_commands.Values;

		/// <summary>
		/// Necessary to prevent reflection errors when inspecting assembly for commands
		/// </summary>
		private static List<string> m_ignoredAssemblies = new List<string>()
		{
			"Microsoft.", "System."
		};

		static CommandManager()
		{
			try
			{
				LoadCommands();
			}
			catch(Exception e)
			{
				Logger.Exception(e);
				Environment.Exit(23);
			}
		}

		/// <summary>
		/// Go through the assembly, find commands and validate them,
		/// and finally construct `CommandData` objects about them
		/// </summary>
		private static void LoadCommands()
		{
			var assemblies = AppDomain.CurrentDomain.GetAssemblies().Where(ass => !m_ignoredAssemblies.Any(ign => ass.FullName.StartsWith(ign))).ToList();
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
				if(method.ReturnType != typeof(string))
				{
					throw new Exception($"Method {method.DeclaringType.Name}:{method.Name} must have a return type of string");
				}
				var attr = method.GetCustomAttribute<CommandAttribute>();
				var cmdData = new CommandData
				{
					Attribute = attr,
					Method = args =>
					{
						if (typeof(Task).IsAssignableFrom(method.ReturnType))
						{
							var t = method.Invoke(null, new object[] { args }) as Task<string>;
							while (!t.IsCompleted)
							{
								Thread.Sleep(10);
							}
							if (t.Exception != null)
							{
								throw t.Exception;
							}
							return t.Result;
						}
						return (string)method.Invoke(null, new object[] { args });
					}
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
			Logger.Debug($"Loaded {m_commands.Count} commands");
		}

		/// <summary>
		/// Route the given arguments to a Command
		/// </summary>
		/// <param name="args">The arguments to be executed</param>
		/// <param name="parameters">The parameters to parse the arguments</param>
		public static string Execute(IEnumerable<string> args, CommandArgumentParameters parameters)
		{
			string result = "";
			try
			{
				// TODO: rejoining strings like this will sometimes result in incorrect result.
				// Edge case, but possible.
				var reconstructedCmdLine = string.Join(new string(parameters.CommandSeperator, 1), args);
				var cmds = m_commands.Where(x => x.Key.IsMatch(reconstructedCmdLine));
				var cmdCount = cmds.Count();
				CommandData cmd;
				if (cmdCount == 1)
				{
					cmd = cmds.Single().Value;
				}
				else
				{
					result += "No matching command found\n";
					Logger.Error("No matching command found");
					var bestMatches = m_commands.ToDictionary(x => x.Value.Attribute,
						x => LevenshteinDistance.Compute(x.Value.Attribute.CommandRegex,
						reconstructedCmdLine.Substring(0, Math.Min(reconstructedCmdLine.Length, x.Value.Attribute.CommandRegex.Length))));
					var bestMatch = bestMatches.Where(x => x.Value <= 5).OrderBy(x => x.Value).FirstOrDefault();
					if(bestMatch.Key != null)
					{
						Logger.Warning($"Did you mean \"{bestMatch.Key.FriendlyName}\"?");
					}
					cmd = m_fallbackCommand;
				}
				var cmdArgs = new CommandArguments(args, parameters);
				OnPreExecute?.Invoke(cmdArgs);
				result += cmd.Method.Invoke(cmdArgs);
			}
			catch (Exception e)
			{
				if (e is AggregateException agg)
				{
					e = agg.InnerException;
				}
				Logger.Exception(e);
				result = $"Exception: {e.InnerException?.Message ?? e.Message}";
			}
			return result;
		}

		public static string Execute(string line, CommandArgumentParameters parameters)
		{
			return Execute(CommandArguments.Split(line, parameters), parameters);
		}

		public static string Execute(string[] args)
		{
			return Execute(args, CommandArgumentParameters.Default);
		}

		public static string Execute(string line)
		{
			return Execute(line, CommandArgumentParameters.Default);
		}

		/// <summary>
		/// This will print a auto-generated help with all registered commands
		/// </summary>
		/// <param name="args"></param>
		//[Command(@"^help$", "help", "List all commands")]
		public static string Help(CommandArguments args)
		{
			if (HelpOverride != null)
			{
				// If the override exists, use that instead				
				return HelpOverride.Invoke();
			}
			Console.WriteLine();
			var consoleWidth = Console.BufferWidth;
			const int margin = 4;
			StringBuilder sb = new StringBuilder();
			foreach (var cmd in m_commands.Where(x => !string.IsNullOrEmpty(x.Value.Attribute.FriendlyName))
				.OrderBy(x => x.Value.Attribute.FriendlyName))
			{
				var friendlyName = cmd.Value.Attribute.FriendlyName;
				var description = cmd.Value.Attribute.Description;
				sb.AppendLine($"{friendlyName}\t........\t{description}");
				if (friendlyName.Length + (margin * 2) + description.Length < consoleWidth)
				{
					var filler = "".PadLeft(consoleWidth - friendlyName.Length - description.Length - (margin * 2), '·');
					var fore = Console.ForegroundColor;
					Console.ForegroundColor = ConsoleColor.DarkGray;
					Console.Write(filler);
					Console.ForegroundColor = fore;
					Console.WriteLine(description);
					Console.WriteLine();
				}
				else
				{
					Console.WriteLine("".PadLeft(margin, ' ') + friendlyName);
					Console.WriteLine("\t\t" + description);
					Console.WriteLine();
				}
			}
			return sb.ToString();
		}

	}
}
