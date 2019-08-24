using Newtonsoft.Json;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CSVTools;

namespace XR.OctoberANFormAnalyzer
{
	class Program
	{
		static void Main(string[] args)
		{
			var responses = new List<XRUserResponse>();
			string[] keys = null;
			using (var csvReader = new CharSeperatedSpreadsheetReader(args[0], '\t'))
			{
				keys = csvReader.Keys;
				foreach(var row in csvReader.Rows)
				{
					responses.Add(new XRUserResponse(row));
				}
			}

			var justNamesAndNumbers = new string[]
			{
				"Name",
				"Phone Number"
			};
			using (var testWriter = new CharSeperatedSpreadsheetWriter("test.tsv", justNamesAndNumbers, '\t'))
			{
				foreach(var response in responses)
				{
					testWriter.WriteRow(() =>
					{
						return new[] { response.Name, response.PhoneNumber };
					});
				}				
			}
		}
	}
}
