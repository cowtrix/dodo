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
		public static string InputPath = "cmdOut.data";
		public static string OutputPath = "cmdIn.data";
		static void Main(string[] args)
		{
			Task outputReader = new Task(() =>
			{
				while (true)
				{
					try
					{
						if (File.Exists(OutputPath))
						{
							using (var stream = GetReadStream(OutputPath, 5 * 1000))
							{
								using (var reader = new StreamReader(stream))
									Console.WriteLine(reader.ReadToEnd());
							}
							File.Delete(OutputPath);
						}
					}
					catch (Exception e)
					{
						Console.WriteLine("CMD:" + e);
					}
					Thread.Sleep(500);
				}
			});
			outputReader.Start();
			while(true)
			{
				var cmd = Console.ReadLine();
				using (var stream = GetWriteStream(InputPath, 5 * 1000))
				{
					using (var writer = new StreamWriter(stream))
						writer.WriteLine(cmd);
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
