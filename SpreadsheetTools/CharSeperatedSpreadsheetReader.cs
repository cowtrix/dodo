using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace CSVTools
{
	public class AliasAttribute : Attribute
	{
		public readonly string Alias;
		public AliasAttribute(string alias)
		{
			Alias = alias;
		}
	}

	public class CharSeperatedSpreadsheetReader : IDisposable
	{
		public class Row
		{
			public Dictionary<string, string> Data = new Dictionary<string, string>();
		}

		public List<Row> Rows = new List<Row>();
		public char Seperator { get; private set; }
		public string[] Keys;
		public CharSeperatedSpreadsheetReader(string path, char seperator = ',')
		{
			Seperator = seperator;
			var lines = File.ReadAllLines(path);
			Keys = lines.First().Split(Seperator);

			for (int rowCounter = 1; rowCounter < lines.Length; rowCounter++)
			{
				string line = (string)lines[rowCounter];
				var row = new Row();
				var columns = line.Split(Seperator);
				if(columns.Length != Keys.Length)
				{
					throw new System.Exception($"Malformed row at line {rowCounter}");
				}
				for (int columnCounter = 0; columnCounter < columns.Length; columnCounter++)
				{
					row.Data[Keys[columnCounter]] = columns[columnCounter];
				}
				Rows.Add(row);
			}
			Console.WriteLine($"Loaded {Rows.Count} entries from {path}");
		}

		public void Dispose()
		{
		}
	}
}
