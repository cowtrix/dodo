using System.Threading.Tasks;
using System.IO;
using System;
using System.Threading;
using System.Collections.Concurrent;
using System.Linq;
using System.Collections;
using System.Collections.Generic;
using System.Text;
using Common;
using Common.Config;

namespace Common
{
	public class CommandReader
	{
		private ConfigVariable<string> OutputPath = new ConfigVariable<string>("CmdOutput", "cmdIn.data");

		private ConfigVariable<string> InputPath = new ConfigVariable<string>("CmdInput", "cmdOut.data");

		public CommandReader()
		{
			var inputThread = new Task(async () =>
			{
				string input = null;
				while (true)
				{
					try
					{
						if (File.Exists(InputPath.Value))
						{
							var allLines = File.ReadAllLines(InputPath.Value);
							File.Delete(InputPath.Value);
							foreach (var line in allLines)
							{
								input = line;
								CommandManager.Execute(line);
							}
						}
					}
					catch (Exception e)
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
			File.WriteAllText(OutputPath.Value, $"{e.Message}");
		}

		private void Output(string text)
		{
			File.WriteAllText(OutputPath.Value, text);
		}

	}
}
