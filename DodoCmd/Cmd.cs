using System;
using System.Collections.Generic;
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
							var output = File.ReadAllText(OutputPath);
							File.Delete(OutputPath);
							Console.WriteLine(output);
						}
					}
					catch (Exception e)
					{
						Console.WriteLine(e);
					}
					Thread.Sleep(500);
				}
			});
			outputReader.Start();
			while(true)
			{
				var cmd = Console.ReadLine();
				File.AppendAllText(InputPath, cmd + "\n");
			}
		}
	}
}
