﻿using Common.Config;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;

namespace DodoCmd
{
	public class Cmd
	{
		private static ConfigVariable<string> OutputPath = new ConfigVariable<string>("CmdOutput", "cmdIn.data");

		private static ConfigVariable<string> InputPath = new ConfigVariable<string>("CmdInput", "cmdOut.data");

		static void Main(string[] args)
		{
			Task outputReader = new Task(async () =>
			{
				while (true)
				{
					try
					{
						if (File.Exists(OutputPath.Value))
						{
							using (var stream = GetReadStream(OutputPath.Value, 5 * 1000))
							{
								using (var reader = new StreamReader(stream))
									Console.WriteLine(reader.ReadToEnd());
							}
							File.Delete(OutputPath.Value);
						}
					}
					catch (Exception e)
					{
						Console.WriteLine("CMD:" + e);
					}
					await Task.Delay(500);
				}
			});
			outputReader.Start();
			while(true)
			{
				var cmd = Console.ReadLine();
				using (var stream = GetWriteStream(InputPath.Value, 5 * 1000))
				{
					using (var writer = new StreamWriter(stream))
						writer.WriteLine(cmd);
				}
				if(cmd == "shutdown")
				{
					Environment.Exit(0);
				}
			}
		}

		public static FileStream GetWriteStream(string path, int timeoutMs)
		{
			var time = Stopwatch.StartNew();
			while (time.ElapsedMilliseconds < timeoutMs)
			{
				try
				{
					return new FileStream(path, FileMode.Create, FileAccess.Write);
				}
				catch (IOException e)
				{
					// access error
					if (e.HResult != -2147024864)
						throw;
				}
			}
			throw new TimeoutException($"Failed to get a write handle to {path} within {timeoutMs}ms.");
		}

		public static FileStream GetReadStream(string path, int timeoutMs)
		{
			var time = Stopwatch.StartNew();
			while (time.ElapsedMilliseconds < timeoutMs)
			{
				try
				{
					return new FileStream(path, FileMode.Open, FileAccess.Read);
				}
				catch (IOException e)
				{
					// access error
					if (e.HResult != -2147024864)
						throw;
				}
			}
			throw new TimeoutException($"Failed to get a write handle to {path} within {timeoutMs}ms.");
		}
	}
}
