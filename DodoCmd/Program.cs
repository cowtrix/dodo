using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace DodoCmd
{
	class Cmd
	{
		static string Path = "cmdOut.txt";
		static void Main(string[] args)
		{
			while(true)
			{
				var cmd = Console.ReadLine();
				File.AppendAllText(Path, cmd);
			}
		}
	}
}
