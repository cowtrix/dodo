using System.Threading.Tasks;
using System.IO;
using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections;
using System.Collections.Generic;

namespace XR.Dodo
{
	public class CmdReader
	{
		public class Command
		{
			public Action<IEnumerable<string>> Action;
		}
		private ConcurrentDictionary<string, Command> m_commands = new ConcurrentDictionary<string, Command>();

		private string OutputPath
		{
			get
			{
				return Path.GetFullPath(DodoCmd.Cmd.OutputPath);
			}
		}

		private string InputPath
		{
			get
			{
				return Path.GetFullPath(DodoCmd.Cmd.InputPath);
			}
		}

		public CmdReader()
		{
			var inputThread = new Task(() =>
			{
				string input = null;
				while(true)
				{
					try
					{
						if(File.Exists(InputPath))
						{
							var allLines = File.ReadAllLines(InputPath);
							File.Delete(InputPath);
							foreach (var line in allLines)
							{
								input = line;
								ProcessCmd(line);
							}
						}
					}
					catch(Exception e)
					{
						Exception(e);
					}
					Thread.Sleep(500);
				}
			});
			inputThread.Start();
		}

		private void Exception(Exception e)
		{
			File.WriteAllText(OutputPath, $"{e.Message}");
		}

		private void ProcessCmd(string line)
		{
			var split = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			var cmdKey = split.First();
			if(!m_commands.TryGetValue(cmdKey, out var command))
			{
				throw new Exception("No matching command found for: " + cmdKey);
			}
			command.Action(split.Skip(1));
		}

		public void AddCommand(string keyPhrase, Action<IEnumerable<string>> action)
		{
			m_commands.TryAdd(keyPhrase, new Command()
			{
				Action = action,
			});
		}
	}
}
