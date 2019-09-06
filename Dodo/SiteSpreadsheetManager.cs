using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace XR.Dodo
{
	public class SiteSpreadsheetManager
	{
		Dictionary<int, SiteSpreadsheet> Sites = new Dictionary<int, SiteSpreadsheet>();
		public SiteSpreadsheetManager(string configPath)
		{
			var configs = File.ReadAllLines(configPath);
			foreach(var config in configs)
			{
				var cols = config.Split('\t');
				if (cols.Length != 3)
				{
					Console.WriteLine($"Failed to read config at line: {config}");
					continue;
				}
				if(!int.TryParse(cols[0], out var sitecode))
				{
					Console.WriteLine($"Failed to parse sitecode at line: {config}");
					continue;
				}
				Sites.Add(sitecode, new SiteSpreadsheet(sitecode, cols[1], cols[2]));
			}
			Console.WriteLine("Finished loading");
		}

		public bool IsCoordinator(string phoneNumber)
		{
			return Sites.Any(x => x.Value.Coordinators.Any(y => y.Number == phoneNumber));
		}
	}
}
