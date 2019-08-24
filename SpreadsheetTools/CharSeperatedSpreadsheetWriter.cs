using System;
using System.IO;
using System.Linq;
using System.Text;

namespace CSVTools
{
	public class CharSeperatedSpreadsheetWriter : IDisposable
	{
		public char Seperator { get; private set; }
		public string[] Keys;

		StringBuilder _stringBuilder = new StringBuilder();
		string _outputPath;
		TextWriter _textWriter;

		public CharSeperatedSpreadsheetWriter(string outputPath, string[] keys, char seperator)
		{
			_outputPath = outputPath;
			Keys = keys;
			Seperator = seperator;
			_textWriter = new StringWriter(_stringBuilder);
			WriteRow(Keys);
		}

		public void WriteRow(string[] row)
		{
			if(row.Length != Keys.Length)
			{
				throw new Exception("Mangled row entry with mismatched column count");
			}
			_textWriter.WriteLine(row.Aggregate("", (current, next) =>
			{
				if(string.IsNullOrEmpty(current))
				{
					return next;
				}
				return $"{current}{Seperator}{next}";
			}));
		}

		public void WriteRow(Func<string[]> row)
		{
			WriteRow(row());
		}

		public void Dispose()
		{
			_textWriter.Dispose();
			File.WriteAllText(_outputPath, _stringBuilder.ToString());
		}
	}
}
