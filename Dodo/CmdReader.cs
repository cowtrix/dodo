﻿using System.Threading.Tasks;
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
			public Func<string, Task> Action;
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
			AddCommand("t", (async x => {
				var response = DodoServer.TelegramGateway.FakeMessage(x, 999999);
				Output(response.Content);
				}));
			AddCommand("s", (async x =>
			{
				var response = DodoServer.SMSGateway.FakeMessage(x, "447856465191");
				Output(response.Content);
			}));
			AddCommand("erroremails", (async x =>
			{
				DodoServer.SiteManager.GenerateErrorEmails(x);
			}));
			AddCommand("backup", (async x =>
			{
				DodoServer.Backup();
			}));
			AddCommand("load", (async x =>
			{
				DodoServer.Load(x);
			}));
			AddCommand("saveconfig", (async x =>
			{
				DodoServer.SaveConfig();
			}));
			AddCommand("shutdown", (async x =>
			{
				DodoServer.Backup();
				Environment.Exit(0);
			}));

			var inputThread = new Task(async () =>
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
					await Task.Delay(500);
				}
			});
			inputThread.Start();
		}

		private void Exception(Exception e)
		{
			File.WriteAllText(OutputPath, $"{e.Message}");
		}

		private void Output(string text)
		{
			File.WriteAllText(OutputPath, text);
		}

		private void ProcessCmd(string line)
		{
			var split = line.Split(new[] { " " }, StringSplitOptions.RemoveEmptyEntries);
			var cmdKey = split.First().ToLowerInvariant();
			if(!m_commands.TryGetValue(cmdKey, out var command))
			{
				throw new Exception("No matching command found for: " + cmdKey);
			}
			var task = new Task(async () => await command.Action(split.Length > 1 ?
				split.Skip(1).Aggregate("", (current, next) => current + " " + next).Trim() : null));
			task.Start();
		}

		public void AddCommand(string keyPhrase, Func<string, Task> action)
		{
			m_commands.TryAdd(keyPhrase, new Command()
			{
				Action = action,
			});
		}
	}
}
